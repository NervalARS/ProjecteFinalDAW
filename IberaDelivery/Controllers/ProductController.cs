using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;




namespace IberaDelivery.Controllers
{
    public class ProductController : Controller
    {
        private readonly iberiadbContext dataContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(iberiadbContext context, IWebHostEnvironment hostEnvironment)
        {
            dataContext = context;
            webHostEnvironment = hostEnvironment;
        }
        // GET: Product
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")) == null) {
                return RedirectToAction("Index", "Home");
            }
            var products = dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .Include(p => p.Images)
            .AsNoTracking();



            return View(await products.ToListAsync());
        }
        [HttpPost]
        public IActionResult Index(String Cadena)
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1) {
                return RedirectToAction("Index", "Home");
            }
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
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1) {
                return RedirectToAction("Index", "Home");
            }
            PopulateCategoriesDropDownList();
            PopulateProvidersDropDownList();
            return View();


        }
        
              /*  private string UploadedFile(FormProduct model)
                {
                    string uniqueFileName = null;

                    if (model.Image != null)
                    {
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName[0];
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {

                            //model.Image.CopyTo(fileStream);
                        }
                    }
                    return uniqueFileName;
                }*/
        
        // POST: Autor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,CategoryId,ProviderId,Stock,Price,Iva,Image")] FormProduct model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1) {
                return RedirectToAction("Index", "Home");
            }
            //if (ModelState.IsValid)
            //{

            Product product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                ProviderId = model.ProviderId,
                Stock = model.Stock,
                Price = model.Price,
                Iva = model.Iva,
            };

            dataContext.Add(product);
            dataContext.SaveChanges();


            foreach (var file in model.Image)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {

                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        Image image = new Image
                        {
                            ProductId = product.Id,
                            Image1 = fileBytes,
                        };
                        dataContext.Add(image);
                        dataContext.SaveChanges();
                        //string s = Convert.ToBase64String(fileBytes);
                        // act on the Base64 data
                    }
                }
            }


            return RedirectToAction(nameof(Index));
            //}
            //else
            //{
            //ViewBag.missatge = product.validarProduct().Missatge;
            //return View();
            //}


        }

        public IActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1) {
                return RedirectToAction("Index", "Home");
            }
            PopulateCategoriesDropDownList();
            PopulateProvidersDropDownList();
            if (id == null)
            {
                return NotFound();
            }

            var product = dataContext.Products
                .Include(p => p.Images)
                .FirstOrDefault(a => a.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            FormProductEdit model = new FormProductEdit
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                ProviderId = product.ProviderId,
                Stock = product.Stock,
                Price = product.Price,
                Iva = product.Iva,
                Image = new List<string>(),
            };

             /*var img = dataContext.Images
            .Where(i => i.ProductId == id)
            .AsNoTracking();*/
            var cont = 0;
            foreach (var file in product.Images)
            {
                if (file.Image1.Length > 0)
                {       
                        model.Image.Add(System.Convert.ToBase64String(file.Image1));     
                }   
            }
            return View(model);
        }

        // POST: Product/Edit/6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Name,Description,CategoryId,ProviderId,Stock,Price,Iva,Image")] FormProduct model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1) {
                return RedirectToAction("Index", "Home");
            }
            //var autor = dataContext.Autors.Find(id);
            //dataContext.Entry(autor).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                dataContext.Update(model);
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

                // POST: Autor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1) {
                return RedirectToAction("Index", "Home");
            }
            var products = dataContext.Products.Find(id);
            if (products != null)
            {
                dataContext.Products.Remove(products);
                dataContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}