using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;




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

        // GET: Producte
        public IActionResult Index()
        {
            var products = dataContext.Products
            .OrderBy(a => a.Id)
            .Include(c => c.Category)
            .Include(p => p.Provider);


            PopulateCategoriesDropDownList();

            return View(products.ToList());
        }


        [HttpPost]
        public IActionResult Index(String Cadena, int cat, int criteri)
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
                    products = products.OrderBy(a => a.Price).ToList();
                    break;
                case 2:
                    products = products.OrderByDescending(a => a.Price).ToList();
                    break;
                case 3:
                    products = products.OrderBy(a => a.Id).ToList();
                    break;
                case 4:
                    products = products.OrderByDescending(a => a.Id).ToList();
                    break;
            }


            ViewBag.missatge = criteri;
            ViewBag.Cadena = Cadena;

            PopulateCategoriesDropDownList();


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
            ViewBag.ProviderId = new SelectList(providers.ToList(), "Id", "FullName", selectedProvider);
        }


        // GET: Producte/Create
        public IActionResult Create()
        {
            PopulateCategoriesDropDownList();
            PopulateProvidersDropDownList();
            return View();


        }

        // POST: Producte/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FormProduct model)
        {

            if (ModelState.IsValid)
            {

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
                //ViewBag.missatge = product.validarProduct().Missatge;
                return View();
            }


        }

        // GET: Producte/Delete/5
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

        // POST: Producte/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var products = dataContext.Products.Find(id);
            if (products != null)
            {
                dataContext.Products.Remove(products);
                dataContext.SaveChanges();
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
        public IActionResult Edit(FormProductEdit model)
        {
            //var autor = dataContext.Autors.Find(id);
            //dataContext.Entry(autor).State = EntityState.Modified;
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
                //ViewBag.missatge = autor.validarAutor().Missatge;
                //return View(model);
                return View();
            }


        }


    }
}
