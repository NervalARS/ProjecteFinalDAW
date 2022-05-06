using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.Json.Serialization;

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
            }
            return View("ShoppingCart", ShoppingCart);
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