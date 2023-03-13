using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductTypesController : Controller
    {
        private readonly AppDbContext context;
        public ProductTypesController(AppDbContext context)
        {
            this.context = context;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(context.ProductTypes.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                context.Add(productTypes);
                await context.SaveChangesAsync();
                TempData["save"] = "Product type has been added successfully";
                return RedirectToAction("Index");
            }
            return View(productTypes);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var productType = context.ProductTypes.Find(id);
            if (productType == null)
                return NotFound();
            return View(productType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                context.Update(productTypes);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productTypes);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            var productType = context.ProductTypes.Find(id);
            if (productType == null)
                return NotFound();
            return View(productType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Details(ProductTypes productTypes)
        {    
            return RedirectToAction("Index");   
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var productType = context.ProductTypes.Find(id);
            if (productType == null)
                return NotFound();
            return View(productType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,ProductTypes productTypes)
        {
            if (id == null)
                return NotFound();

            if (id != productTypes.Id)
            {
                return NotFound();
            }

            var productType = context.ProductTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                context.Remove(productType);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productTypes);
        }
    }
}
