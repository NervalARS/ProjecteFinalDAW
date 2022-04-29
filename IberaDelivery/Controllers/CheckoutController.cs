using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace IberaDelivery.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly iberiadbContext dataContext;
        public CheckoutController(iberiadbContext context)
        {
            dataContext = context;
        }
        public async Task<IActionResult> Checkout()
        {
            List<Product> list;
            list = new List<Product>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                list = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            }
            var orders = dataContext.Orders;
            DateTime today = DateTime.Today;
            var order = new Order();
            order.Date = today;
            order.Import = 0;
            order.UserId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;
            dataContext.Add(order);
            dataContext.SaveChanges();
            for (var i = 0; i < list.Count; i++)
            {
                var lnOrder = new LnOrder();
                lnOrder.NumOrder = order.Id;
                lnOrder.RefProduct = list[i].Id;
                lnOrder.Quantity = list[i].Stock;
                lnOrder.TotalImport = list[i].Price + list[i].Iva;
                var product = dataContext.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Provider)
                .FirstOrDefault(a => a.Id == list[i].Id);
                if (product.Stock > lnOrder.Quantity)
                {
                    dataContext.Add(lnOrder);
                    dataContext.SaveChanges();
                    product.Stock = product.Stock - lnOrder.Quantity;
                    dataContext.Update(product);
                    dataContext.SaveChanges();
                }
            }
            var products = dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .AsNoTracking();
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Index", "Product", await products.ToListAsync());
            //return View("Index", await products.ToListAsync());
        }
    }
}