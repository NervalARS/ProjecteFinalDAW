using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;




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
            try
            {
                var products = dataContext.Products
                .OrderBy(a => a.Id)
                .Include(c => c.Category)
                .Include(p => p.Provider);
                PopulateCategoriesDropDownList();
                return View(products.ToList());
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }


        }
        [HttpPost]
        public IActionResult Index(String Cadena, int cat, int criteri)
        {
            try
            {
                var products = dataContext.Products
             .Include(c => c.Category)
             .Include(p => p.Provider).ToList();
                if (Cadena != null || cat != 0)
                {
                    if (Cadena == null)
                    {
                        products = dataContext.Products
                        .Where(a => a.CategoryId == cat)
                       .Include(c => c.Category)
                       .Include(p => p.Provider).ToList();

                    }
                    else
                    {
                        if (cat != 0)
                        {
                            products = dataContext.Products
                             .Where(a => a.Name.Contains(Cadena) && a.CategoryId == cat)
                            .Include(c => c.Category)
                            .Include(p => p.Provider).ToList();

                        }
                        else
                        {
                            products = dataContext.Products
                            .Where(a => a.Name.Contains(Cadena))
                            .Include(c => c.Category)
                            .Include(p => p.Provider).ToList();
                        }
                    }
                }
                switch (criteri)
                {
                    case 1:
                        products = products.OrderBy(a => a.Id).ToList();
                        break;
                    case 2:
                        products = products.OrderByDescending(a => a.Id).ToList();
                        break;
                    case 3:
                        products = products.OrderBy(a => a.Price).ToList();
                        break;
                    case 4:
                        products = products.OrderByDescending(a => a.Price).ToList();
                        break;
                }
                ViewBag.missatge = criteri;
                ViewBag.Cadena = Cadena;

                PopulateCategoriesDropDownList();


                return View(products.ToList());
            }
            catch (Exception e)
            {
                return RedirectToAction("Error500", "Home");
            }

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
            return View();
        }
        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FormProduct model)
        {
            try
            {
                if (!checkUserExists() || checkUserIsClient())
                {
                    return RedirectToAction("Index", "Home");
                }
                Product product = new Product
                {
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    ProviderId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id,
                    Stock = model.Stock,
                    Price = model.Price,
                    Iva = model.Iva,
                };
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
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }
        }

        // GET: Product/Detail/5
        public IActionResult Detail(int? id)
        {
            var product = buscarProducte(id);
            ViewBag.Users = dataContext.Users;
            return View(product);

        }
        // GET: Product/Delete/5
        public IActionResult Delete(int? id)
        {
            try
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
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }

        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
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
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

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
            try
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
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }

        public IActionResult Edit(int? id)
        {
            try
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
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }

        // POST: Producte/Edit/6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FormProductEdit model)
        {
            try
            {
                if (ModelState.IsValid)
                {
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
                                    //string s = Convert.ToBase64String(fileBytes);
                                    // act on the Base64 data
                                }
                            }
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }
        /*
            Shopping Cart Methods
            Si me da tiempo los quito, si no, no lo hare
        */

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
            var product = buscarProducte(id);
            var oldStock = product.Stock;
            if (product != null)
            {
                if (list.FirstOrDefault(a => a.Id == id) != null)
                {
                    var pr = list.FirstOrDefault(a => a.Id == id);
                    if (product.Stock > pr.Stock)
                    {
                        pr.Stock = pr.Stock + 1;
                        pr.Price = (pr.Price + product.Price);
                        pr.Iva = (pr.Iva + product.Iva);
                        list.Remove(list.FirstOrDefault(a => a.Id == id));
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
            product.Stock = oldStock;
            return RedirectToAction("Detail", new { id });
        }
        /*
            Functions that dont return views
        */
        public Product buscarProducte(int? id)
        {
            var product = dataContext.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Provider)
                .Include(p => p.Comments)
                .FirstOrDefault(a => a.Id == id);
            foreach (var image in product.Images)
            {
                image.Product = null;
            }
            foreach (var comment in product.Comments)
            {
                comment.Product = null;
                comment.User = null;
            }
            product.Category.Products = null;
            product.Provider.Products = null;
            return product;
        }
        public bool checkUserExists()
        {
            // If (user == null) return false
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return false;
            }
            // else return true
            return true;
        }
        public bool checkUserIsClient()
        {
            // If (rol == 3) return true
            if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 3)
            {
                return true;
            }
            // Else return false;
            return false;
        }
        public bool checkUserIsProveidor()
        {
            // If (rol == 2) return true
            if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 2)
            {
                return true;
            }
            // Else return false;
            return false;
        }
        public bool checkUserIsAdmin()
        {
            // If (rol == 1) return true
            if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 1)
            {
                return true;
            }
            // Else return false;
            return false;
        }
    }
}