using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Interfaces;
using OnlineShop.Models;
using OnlineShop.ViewModel;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext context;
        private readonly IPhotoService PhotoService;

        public ProductController(AppDbContext context, IPhotoService PhotoService)
        {
            this.context = context;
            this.PhotoService = PhotoService;
        }
        public IActionResult Index()
        {
            return View(context.Products.Include(c => c.ProductType).Include(f => f.SpecialTag).ToList());
        }
        //range product price
        [HttpPost]
        public IActionResult Index(decimal? lowPrice, decimal? HighPrice)
        {
            var products = context.Products.Include(c => c.ProductType).Include(f => f.SpecialTag).Where(p => p.Price >= lowPrice && p.Price <= HighPrice).ToList();

            if(lowPrice == null || HighPrice == null)
            {
                products = context.Products.Include(c => c.ProductType).Include(f => f.SpecialTag).ToList();
            }
            return View(products);
        }
        public IActionResult Create()
        {
            ViewData["productTypeId"] = new SelectList(context.ProductTypes.ToList(),"Id","ProductType");
            ViewData["TagId"] = new SelectList(context.SpecialTag .ToList(),"Id","Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel ProductVsM)
        {
            if (ModelState.IsValid)
            {              
                var result = await PhotoService.UploadPhotoAsync(ProductVsM.Image);
                var products = new Products
                {
                    Name = ProductVsM.Name,
                    Price = ProductVsM.Price,
                    Image = result.Url.ToString(),
                    ProductColor = ProductVsM.ProductColor,
                    IsAvailable = ProductVsM.IsAvailable,
                    ProductTypeId = ProductVsM.ProductTypeId,
                    SpecialTagId = ProductVsM.SpecialTagId
                };
                //chek if product name already exist
                var searchProduct = context.Products.FirstOrDefault(c => c.Name == products.Name);
                if (searchProduct != null)
                {
                    ViewBag.message = "this product already exist";
                    ViewData["productTypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "ProductType");
                    ViewData["TagId"] = new SelectList(context.SpecialTag.ToList(), "Id", "Name");
                    return View(ProductVsM);
                }
                context.Products.Add(products);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ProductVsM);
        }
        public IActionResult Edit(int? id)
        {
            ViewData["productTypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "ProductType");
            ViewData["TagId"] = new SelectList(context.SpecialTag.ToList(), "Id", "Name");
            if (id == null)
                return NotFound();
            var product = context.Products.Include(c => c.ProductType).Include(f => f.SpecialTag).FirstOrDefault(c => c.Id == id);

            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,ProductViewModel ProductVM)
        {
            if (id == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                var product = context.Products.Include(c => c.ProductType).Include(f => f.SpecialTag).FirstOrDefault(c => c.Id == id);
                if(product != null)
                {
                    var result = product.Image;
                    if(ProductVM.Image != null)
                    {
                        try
                        {
                            await PhotoService.DeletePhotoAsync(product.Image);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Cant to delete photo");
                            return View(ProductVM);
                        }
                        var uploadRes = await PhotoService.UploadPhotoAsync(ProductVM.Image);
                        result = uploadRes.Url.ToString();
                    }
                   
                    
                    var products = new Products
                    {
                        Id = id,
                        Name = ProductVM.Name,
                        Price = ProductVM.Price,
                        Image = result,
                        ProductColor = ProductVM.ProductColor,
                        IsAvailable = ProductVM.IsAvailable,
                        ProductTypeId = ProductVM.ProductTypeId,
                        SpecialTagId = ProductVM.SpecialTagId
                    };
                    context.Update(products);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }
            return View(ProductVM);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            ViewData["productTypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "ProductType");
            ViewData["TagId"] = new SelectList(context.SpecialTag.ToList(), "Id", "Name");
            if (id == null)
                return NotFound();
            var product = context.Products.Include(c => c.ProductType).Include(f => f.SpecialTag).FirstOrDefault(c => c.Id == id);

            return View(product);
        }

        [HttpPost]
        public IActionResult Details(Products products)
        {
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            ViewData["productTypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "ProductType");
            ViewData["TagId"] = new SelectList(context.SpecialTag.ToList(), "Id", "Name");
            if (id == null)
                return NotFound();
            var product = context.Products.Include(c => c.ProductType).Include(f => f.SpecialTag).FirstOrDefault(c => c.Id == id);

            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, Products products)
        {
            if (id == null)
                return NotFound();
            if (id != products.Id)
                return NotFound();
            var product = context.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
                return NotFound();
            try
            {
                await PhotoService.DeletePhotoAsync(product.Image);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Cant to delete photo");
                return View(products);
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
