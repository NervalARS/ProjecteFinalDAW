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
        public bool checkUserExists()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return false;
            }
            return true;
        }
        public bool checkUserIsClient()
        {
            if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 3)
            {
                return true;
            }
            return false;
        }
        public bool checkUserIsProveidor()
        {
            if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 2)
            {
                return true;
            }
            return false;
        }
        public bool checkUserIsAdmin()
        {
            if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 1)
            {
                return true;
            }
            return false;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            if (!checkUserExists())
            {
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
            if (!checkUserExists())
            {
                return RedirectToAction("Index", "Home");
            }
            var products = dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .Include(p => p.Images)
            .Where(p => p.Name.Contains(Cadena)); //|| a.Cognoms.Contains(Cadena));
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
            var providers = dataContext.Users
            .Where(u => u.Rol.Equals(2));
            ViewBag.ProviderId = new SelectList(providers.ToList(), "Id", "FirstName", selectedProvider);
        }
        // GET: Product/Create
        public IActionResult Create()
        {
            if (!checkUserExists() || checkUserIsClient())
            {
                return RedirectToAction("Index", "Home");
            }
            PopulateCategoriesDropDownList();
            PopulateProvidersDropDownList();
            if (checkUserIsProveidor())
            {
                ViewBag.ThisProviderId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;
                ViewBag.ThisProviderName = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).FirstName;
            }
            return View();
        }
        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,CategoryId,ProviderId,Stock,Price,Iva,Image")] FormProduct model)
        {
            if (!checkUserExists() || checkUserIsClient())
            {
                return RedirectToAction("Index", "Home");
            }

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

            if (model.Image != null)
            {
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
                        }
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Delete/5
        public IActionResult Delete(int? id)
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

        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var products = dataContext.Products.Find(id);
            if (products != null)
            {
                dataContext.Products.Remove(products);
                try
                {
                    dataContext.SaveChanges();
                }
                catch (System.Exception)
                {
                    ViewBag.errors = "problema con el datacontext";
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var image = dataContext.Images
                .FirstOrDefault(a => a.Id == id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: Producte/DeleteImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteImage(int id, string url)
        {
            var image = dataContext.Images.Find(id);
            /* var image = dataContext.Images
             .Where(p => p.ProductId == id);*/
            if (image != null)
            {
                dataContext.Images.Remove(image);
                dataContext.SaveChanges();
            }

            return Redirect("/" + url);
        }

        public IActionResult Edit(int? id)
        {
            if (!checkUserExists() || checkUserIsClient())
            {
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
                Image = new List<Image>(),
            };
            var cont = 0;
            foreach (var file in product.Images)
            {
                if (file.Image1.Length > 0)
                {
                    Image img = new Image
                    {
                        Id = file.Id,
                        ProductId = file.ProductId,
                        Image1 = file.Image1
                    };

                    //model.Image.Add(System.Convert.ToBase64String(file.Image1));
                    model.Image.Add(img);
                }
            }
            return View(model);
        }

        // POST: Producte/Edit/6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Name,Description,CategoryId,ProviderId,Stock,Price,Iva,ImageIn")] FormProductEdit model)
        {
            if (!checkUserExists() || checkUserIsClient())
            {
                return RedirectToAction("Index", "Home");
            }
            Product product = new Product
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                ProviderId = model.ProviderId,
                Stock = model.Stock,
                Price = model.Price,
                Iva = model.Iva,
            };
            dataContext.Update(product);
            dataContext.SaveChanges();

            if (model.ImageIn != null)
            {
                foreach (var file in model.ImageIn)
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
                        }
                    }
                }
            }
            return RedirectToAction(nameof(Index));
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
                lnOrder.TotalImport = list[i].Price;

                var product = dataContext.Products
                .FirstOrDefault(p => p.Id == list[i].Id);

                if (product.Stock > lnOrder.Quantity){
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
            return View("Index", await products.ToListAsync());
        }

        public async Task<IActionResult> AddToCart(int? id)
        {
            List<Product> list;
            if (HttpContext.Session.GetString("Cart") == null)
            {
                list = new List<Product>();
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
                var tempStock = 0;
            }
            else
            {
                list = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            }
            var product = dataContext.Products
                .FirstOrDefault(a => a.Id == id);
            if (product != null)
            {
                if (list.FirstOrDefault(a => a.Id == id) != null){
                    var pr = list.FirstOrDefault(product);
                    pr.Stock = pr.Stock+1;
                    pr.Price = (pr.Price + product.Price);
                    list.Remove(list.FirstOrDefault(product));
                    list.Add(pr);
                } else {
                    product.Stock = 1;
                    list.Add(product);
                }
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
    }
}