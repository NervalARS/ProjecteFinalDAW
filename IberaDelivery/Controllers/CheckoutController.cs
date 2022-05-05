using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Index()
        {
            List<Product> ShoppingCart;
            ShoppingCart = new List<Product>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                ShoppingCart = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
                ViewBag.Products = ShoppingCart;
            }
            return View("ShoppingCart", ShoppingCart);
        }
        private void PopulateShipmentsDropDownList(object? selectedShipment = null)
        {
            var UserId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;
            var shipments = dataContext.Shipments.Where(a => a.UserId == UserId);
            ViewBag.ShipmentId = new SelectList(shipments.ToList(), "Id", "Address", selectedShipment);
        }

        private void PopulateProductsList(object? selectedShipment = null)
        {
            List<Product> ShoppingCart;
            ShoppingCart = new List<Product>();
            ShoppingCart = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            ViewBag.Products = ShoppingCart;
        }

        public async Task<IActionResult> CheckoutDetails()
        {
            PopulateShipmentsDropDownList();
            PopulateProductsList();
            ViewBag.User = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));
            return View("Checkout");
        }
        public async Task<IActionResult> Checkout(CheckoutForm model)
        {
            // Generamos variables
            var orders = dataContext.Orders;
            DateTime today = DateTime.Today;
            var order = new Order();
            List<Product> ShoppingCart;
            //ShoppingCart = new List<Product>();
            ShoppingCart = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            var User = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));
            // Setteamos la información de Order
            order.Date = today;
            order.Import = 0;
            order.UserId = User.Id;
            order.ShipmentId = model.ShipmentId;
            // Añadimos la linea de Order
            dataContext.Add(order);
            dataContext.SaveChanges();
            foreach (var item in ShoppingCart)
            {
                var lnOrder = new LnOrder();
                lnOrder.NumOrder = order.Id;
                lnOrder.RefProduct =item.Id;
                lnOrder.Quantity = item.Stock;
                lnOrder.TotalImport = item.Price + item.Iva;
                var product = dataContext.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Provider)
                .FirstOrDefault(a => a.Id == item.Id);
                if (product.Stock > lnOrder.Quantity)
                {
                    dataContext.Add(lnOrder);
                    dataContext.SaveChanges();
                    product.Stock = product.Stock - lnOrder.Quantity;
                    dataContext.Update(product);
                    dataContext.SaveChanges();
                }
            }
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AddToCart(int? id, String? src)
        {
            List<Product> list;
            if (HttpContext.Session.GetString("Cart") == null)
            {
                list = new List<Product>();
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            else
            {
                list = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            }
            var product = buscarProducte(id);
            var oldStock = product.Stock;
            if (product != null)
            {
                if (list.FirstOrDefault(a => a.Id == id) != null)
                {
                    var pr = list.FirstOrDefault(a => a.Id == id);
                    pr.Stock = pr.Stock + 1;
                    pr.Price = (pr.Price + product.Price);
                    pr.Iva = (pr.Iva + product.Iva);
                    list.Remove(list.FirstOrDefault(a => a.Id == id));
                    list.Add(pr);
                }
                else
                {
                    product.Stock = 1;
                    list.Add(product);
                }
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            if (product != null)
            {
                product.Stock = oldStock;
            }

            return RedirectToAction("Search", "Navbar", new { src, product.Category.Id });
        }

        public async Task<IActionResult> AddOne(int? id)
        {
            List<Product> list;
            if (HttpContext.Session.GetString("Cart") == null)
            {
                list = new List<Product>();
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            else
            {
                list = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            }
            var product = buscarProducte(id);
            var oldStock = product.Stock;
            if (product != null)
            {
                if (list.FirstOrDefault(a => a.Id == id) != null)
                {
                    var pr = list.FirstOrDefault(a => a.Id == id);
                    pr.Stock = pr.Stock + 1;
                    pr.Price = (pr.Price + product.Price);
                    pr.Iva = (pr.Iva + product.Iva);
                    list.Remove(list.FirstOrDefault(a => a.Id == id));
                    list.Add(pr);
                }
                else
                {
                    product.Stock = 1;
                    list.Add(product);
                }
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            if (product != null)
            {
                product.Stock = oldStock;
            }
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> RemoveOne(int? id)
        {
            List<Product> list;
            if (HttpContext.Session.GetString("Cart") == null)
            {
                list = new List<Product>();
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            else
            {
                list = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            }
            var product = buscarProducte(id);
            var oldStock = product.Stock;
            if (product != null)
            {
                if (list.FirstOrDefault(a => a.Id == id) != null)
                {
                    var pr = list.FirstOrDefault(a => a.Id == id);
                    pr.Stock = pr.Stock - 1;
                    pr.Price = (pr.Price - product.Price);
                    pr.Iva = (pr.Iva - product.Iva);
                    list.Remove(list.FirstOrDefault(a => a.Id == id));
                    if (pr.Stock > 0)
                    {
                        list.Add(pr);
                    }
                }
                else
                {
                    product.Stock = 1;
                    list.Add(product);
                }
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            if (product != null)
            {
                product.Stock = oldStock;
            }
            return RedirectToAction("Index");

        }



        public async Task<IActionResult> ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return View("Index");
        }
        public Product buscarProducte(int? id)
        {
            var product = dataContext.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Provider)
                .Include(p => p.Comments)
                .FirstOrDefault(a => a.Id == id);
            if (product.Images != null)
            {
                foreach (var image in product.Images)
                {
                    image.Product = null;
                }
            }
            if (product.Comments != null)
            {
                foreach (var comment in product.Comments)
                {
                    comment.Product = null;
                    comment.User = null;
                }
            }
            product.Category.Products = null;
            product.Provider.Products = null;
            return product;
        }
    }

}