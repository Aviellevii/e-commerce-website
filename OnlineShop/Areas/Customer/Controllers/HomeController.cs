using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;
using System.Diagnostics;
using X.PagedList;

namespace OnlineShop.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly AppDbContext context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index(int? page)
        {
            return View(context.Products.Include(c=>c.ProductType).Include(f=> f.SpecialTag).ToList().ToPagedList(page??1,9)) ;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int? id)
        {
            ViewData["productTypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "ProductType");
            if (id == null)
                return NotFound();

            var product = context.Products.Include(c => c.ProductType).FirstOrDefault(p=>p.Id == id);

            return View(product);
        }
        [HttpPost]
        [ActionName("Details")]
        public IActionResult ProductDetail(int? id)
        {
            List<Products> products = new List<Products>();
            if (id == null)
                return NotFound();

            var product = context.Products.Include(c => c.ProductType).FirstOrDefault(p => p.Id == id);

            products = HttpContext.Session.Get<List<Products>>("products");

            if(products == null)
            {
                products = new List<Products>();
            }
            
            products.Add(product);
            HttpContext.Session.Set("products", products);
            return RedirectToAction("Index");
        }
    

        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Products>products = HttpContext.Session.Get<List<Products>>("products");

            if (products != null)
            {
                var product = products.FirstOrDefault(p => p.Id == id);
                if(product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);

                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Cart()
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");

            if (products == null)
                products = new List<Products>();
            return View(products);
        }

        [ActionName("Remove")]
        public IActionResult RemoveFromCart(int? id)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");

            if (products != null)
            {
                var product = products.FirstOrDefault(p => p.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);

                }
            }
            return View("Index");
        }
    }
}