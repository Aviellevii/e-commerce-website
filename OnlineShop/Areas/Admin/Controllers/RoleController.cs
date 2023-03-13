using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Areas.Admin.Models;
using OnlineShop.Data;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        AppDbContext _context;
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var role = _roleManager.Roles.ToList();
            ViewBag.Roles = role;
            return View();
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            IdentityRole role = new IdentityRole();
            role.Name = name;
            var roleExist = await _roleManager.RoleExistsAsync(role.Name);
            if(roleExist)
            {
                ViewBag.msg = "This Role Already Exist";
                ViewBag.name = name;
                return View();
            }
            var result = await _roleManager.CreateAsync(role);
            if(result.Succeeded)
            {
                TempData["Save"] = "Role has been saved";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            ViewBag.id = role.Id;
            ViewBag.name = role.Name;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id,string name)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            role.Name = name;
            var roleExist = await _roleManager.RoleExistsAsync(role.Name);
            if (roleExist)
            {
                ViewBag.msg = "This Role Already Exist";
                ViewBag.name = name;
                return View();
            }
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                TempData["Save"] = "Role has been updated";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            ViewBag.id = role.Id;
            ViewBag.name = role.Name;
            return View();
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Save"] = "Role has been Deleted";
                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> Assign()
        {
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers.Where(f=>f.LockoutEnd<DateTime.Now||f.LockoutEnd==null).ToList(), "Id", "UserName");
            ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Assign(RoleUserVM roleuser)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == roleuser.UserId);
            var isCheckRoleAssign = await _userManager.IsInRoleAsync(user,roleuser.RoleId);
            if(isCheckRoleAssign)
            {
                ViewBag.msg = "The user is already Assigned";
                ViewData["UserId"] = new SelectList(_context.ApplicationUsers.Where(f => f.LockoutEnd < DateTime.Now || f.LockoutEnd == null).ToList(), "Id", "UserName");
                ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
                return View();
            }
            if (user == null)
                return NotFound();
            var role = await _userManager.AddToRoleAsync(user, roleuser.RoleId);
            if(role.Succeeded)
            {
                TempData["save"] = "The user Assigned";
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult AssignRoleUser()
        {
            var result = from ur in _context.UserRoles
                         join r in _context.Roles on ur.RoleId equals r.Id
                         join a in _context.ApplicationUsers on ur.UserId equals a.Id
                         select new UserRoleMapping()
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserName = a.UserName,
                             RoleName = r.Name
                         };
            ViewBag.UserRoles = result;
            return View();
        }
    }
}
