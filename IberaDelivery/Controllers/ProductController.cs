using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IberaDelivery.Controllers
{
    public class ProductController : Controller
    {
        private readonly iberiadbContext dataContext;

        public ProductController(iberiadbContext context)
        {
            dataContext = context;
        }
        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .AsNoTracking();
            return View(await products.ToListAsync());
        }
        [HttpPost]
        public IActionResult Index(String Cadena)
        {
            var products = dataContext.Products
            .Where(a => a.Name.Contains(Cadena)); //|| a.Cognoms.Contains(Cadena));
            ViewBag.missatge = "Filtrat per: " + Cadena;
            return View(products.ToList());
        }
        private void PopulateCategoriesDropDownList(object? selectedCategory = null)
        {
            var categories = dataContext.Categories;
            ViewBag.CategoryId = new SelectList(categories.ToList(), "Id", "Name", selectedCategory);
        }
        private void PopulateProvidersDropDownList(object? selectedProvider = null)
        {
            var providers = dataContext.Users;
            ViewBag.ProviderId = new SelectList(providers.ToList(), "Id", "FirstName", selectedProvider);
        }
        // GET: Product/Create
        public IActionResult Create()
        {
            PopulateCategoriesDropDownList();
            PopulateProvidersDropDownList();
            return View();
        }
        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,CategoryId,ProviderId,Stock,Price,Iva")] Product product)
        {
            //if (ModelState.IsValid)
            //{
                dataContext.Add(product);
                dataContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            //}
            //else
            //{
                //ViewBag.missatge = product.validarProduct().Missatge;
                //return View();
            //}
        }
        // GET: Product/Delete/5
        public IActionResult Delete(int? id)
        {
            if (HttpContext.Session.GetString("userName") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var product = dataContext.Products
                    .FirstOrDefault(a => a.Id == id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            else
            {
                return Redirect("/");
            }
        }
        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = dataContext.Products.Find(id);
            if (product != null)
            {
                dataContext.Products.Remove(product);
                dataContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            PopulateCategoriesDropDownList();
            PopulateProvidersDropDownList();
            if (id == null)
            {
                return NotFound();
            }
            var product = dataContext.Products
                .FirstOrDefault(a => a.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Name,Description,CategoryId,ProviderId,Stock,Price,Iva")] Product product)
        {
            //var autor = dataContext.Autors.Find(id);
            //dataContext.Entry(autor).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                dataContext.Update(product);
                dataContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //ViewBag.missatge = autor.validarAutor().Missatge;
                return View();
            }
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = dataContext.Products
                .FirstOrDefault(a => a.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        public async Task<IActionResult> AddToCart(int? id)
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
            var product = dataContext.Products
                .FirstOrDefault(a => a.Id == id);
            if (product != null)
            {
                list.Add(product);
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            var products = dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .AsNoTracking();
            return View("Index", await products.ToListAsync());
        }

                public async Task<IActionResult> ClearCart(int? id)
        {
            HttpContext.Session.Remove("Cart");
            var products = dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .AsNoTracking();
            return View("Index", await products.ToListAsync());
        }

        public async Task<IActionResult> Checkout(){
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
            order.UserId = 1;
            dataContext.Add(order);
            dataContext.SaveChanges();

            for (var i = 0; i < list.Count; i++) {
                var lnOrder = new LnOrder(); 
                lnOrder.NumOrder = order.Id;
                //lnOrder.NumLine = i;
                lnOrder.RefProduct = list[i].Id;
                lnOrder.Quantity = 1;
                lnOrder.TotalImport = list[i].Price;
                dataContext.Add(lnOrder);
                dataContext.SaveChanges();
            }
            
            var products = dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .AsNoTracking();
            return View("Index", await products.ToListAsync());
        }
    }
}