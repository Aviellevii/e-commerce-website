using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        UserManager<IdentityUser> _userManager;
        private readonly AppDbContext context;
        public UserController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            this.context = context;
        }
        public IActionResult Index()
        {
            return View(context.ApplicationUsers.ToList());
        }
        public async Task<IActionResult>Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if(ModelState.IsValid)
            {
              if(ModelState.IsValid)
                {
                    var result = await _userManager.CreateAsync(user, user.PasswordHash);
                    if (result.Succeeded)
                    {
                        var isSaveRole = await _userManager.AddToRoleAsync(user, "User");
                        TempData["save"] = "User has been created successfully";
                        return RedirectToAction("Index");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            var user = context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var userInfo = context.ApplicationUsers.FirstOrDefault(u => u.Id == user.Id);
            if (userInfo == null)
                return NotFound();
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            var result = await _userManager.UpdateAsync(userInfo);

            if(result.Succeeded)
            {
                TempData["save"] = "User has been Updated successfully";
                return RedirectToAction("Index");
            }
            return View(userInfo);
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();
            var user = context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        public async Task<IActionResult> Locout(string id)
        {
            if (id == null)
                return NotFound();
            var user = context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Locout(ApplicationUser user)
        {
            var userInfo =  context.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();

            }
            userInfo.LockoutEnd = DateTime.Now.AddYears(100);
            var result = await _userManager.UpdateAsync(userInfo);

            if (result.Succeeded)
            {
                TempData["save"] = "User has been Lockouted successfully";
                return RedirectToAction("Index");
            }
            return View(userInfo);
        }

        public async Task<IActionResult> Active(string id)
        {
            if (id == null)
                return NotFound();
            var user = context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Active(ApplicationUser user)
        {
            var userInfo = context.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();

            }
            userInfo.LockoutEnd = DateTime.Now.AddDays(-1);
            var result = await _userManager.UpdateAsync(userInfo);

            if (result.Succeeded)
            {
                TempData["save"] = "User has been Activated successfully";
                return RedirectToAction("Index");
            }
            return View(userInfo);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();
            var user = context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser user)
        {
            var userInfo = context.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();

            }
            context.ApplicationUsers.Remove(userInfo);
            TempData["save"] = "User has been Deleted successfully";
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
