using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecialTagController : Controller
    {
        private readonly AppDbContext context;
        public SpecialTagController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View(context.SpecialTag.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                context.Add(specialTag);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(specialTag);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var specialTag = context.SpecialTag.Find(id);
            if (specialTag == null)
                return NotFound();
            return View(specialTag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                context.Update(specialTag);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(specialTag);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            var specialTag = context.SpecialTag.Find(id);
            if (specialTag == null)
                return NotFound();
            return View(specialTag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(SpecialTag specialTag)
        {
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var specialTag = context.SpecialTag.Find(id);
            if (specialTag == null)
                return NotFound();
            return View(specialTag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, SpecialTag specialTag)
        {
            if (id == null)
                return NotFound();

            if (id != specialTag.Id)
            {
                return NotFound();
            }

            var specialTags = context.SpecialTag.Find(id);
            if (specialTags == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                context.Remove(specialTags);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(specialTag);
        }
    }
}
