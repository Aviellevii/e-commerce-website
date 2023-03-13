using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly AppDbContext context;
        public OrderController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order anOrder)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if(products != null)
            {
                foreach(var product in products)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.ProductId = product.Id;
                    anOrder.OrderDetails.Add(orderDetails);
                }
            }
            anOrder.OrderNo = GetOrderNo();
            context.Orders.Add(anOrder);
            await context.SaveChangesAsync();
            TempData["save"] = "The order saved";
            HttpContext.Session.Set("products", new List<Products>());
            return View();
        }
        public string GetOrderNo()
        {
            int rowCount = context.Orders.ToList().Count() + 1;
            return rowCount.ToString("000");
        }


    }
}
