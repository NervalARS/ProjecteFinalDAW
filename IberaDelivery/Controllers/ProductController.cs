using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IberaDelivery.Controllers
=======
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pt1_mvc.Controllers
>>>>>>> origin/pol
{
    public class ProductController : Controller
    {
        private readonly iberiadbContext dataContext;

        public ProductController(iberiadbContext context)
        {
            dataContext = context;
        }

<<<<<<< HEAD
        // GET: Product
=======
        // GET: Autor
>>>>>>> origin/pol
        public async Task<IActionResult> Index()
        {
            var products = dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .AsNoTracking();
            return View(await products.ToListAsync());
<<<<<<< HEAD

        }

=======
        }


>>>>>>> origin/pol
        [HttpPost]
        public IActionResult Index(String Cadena)
        {

            var products = dataContext.Products
<<<<<<< HEAD
            .Where(a => a.Name.Contains(Cadena)); //|| a.Cognoms.Contains(Cadena));
=======
            .Where(p => p.Name.Contains(Cadena)); //|| a.Cognoms.Contains(Cadena));
>>>>>>> origin/pol
            ViewBag.missatge = "Filtrat per: " + Cadena;

            return View(products.ToList());

        }

<<<<<<< HEAD
        private void PopulateCategoriesDropDownList(object? selectedCategory = null)
=======
        private void PopulateCategoriesDropDownList(object selectedCategory = null)
>>>>>>> origin/pol
        {
            var categories = dataContext.Categories;
            ViewBag.CategoryId = new SelectList(categories.ToList(), "Id", "Name", selectedCategory);
        }

<<<<<<< HEAD
        private void PopulateProvidersDropDownList(object? selectedProvider = null)
=======
        private void PopulateProvidersDropDownList(object selectedProvider = null)
>>>>>>> origin/pol
        {
            var providers = dataContext.Users;
            ViewBag.ProviderId = new SelectList(providers.ToList(), "Id", "FullName", selectedProvider);
        }

<<<<<<< HEAD
        // GET: Product/Create
        public IActionResult Create()
        {
            PopulateCategoriesDropDownList();
            PopulateProvidersDropDownList();
                return View();
           
           
        }

        // POST: Product/Create
=======

        // GET: Autor/Create
        public IActionResult Create()
        {
           PopulateCategoriesDropDownList();
           PopulateProvidersDropDownList();
                return View();
            
            
        }

        // POST: Autor/Create
>>>>>>> origin/pol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,CategoryId,ProviderId,Stock,Price,Iva")] Product product)
        {

<<<<<<< HEAD
            if (ModelState.IsValid)
            {
                dataContext.Add(product);
                dataContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //ViewBag.missatge = autor.validarAutor().Missatge;
                return View();
            }
=======
           
                dataContext.Add(product);
                dataContext.SaveChanges();
                return RedirectToAction(nameof(Index));
           
>>>>>>> origin/pol


        }

        // GET: Autor/Delete/5
        public IActionResult Delete(int? id)
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("userName") != null)
            {
=======
            
>>>>>>> origin/pol
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
<<<<<<< HEAD
            }
            else
            {
                return Redirect("/");
            }
=======
            
           
            
>>>>>>> origin/pol
        }

        // POST: Autor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
<<<<<<< HEAD
            var product = dataContext.Products.Find(id);
            if (product != null)
            {
                dataContext.Products.Remove(product);
=======
            var products = dataContext.Products.Find(id);
            if (products != null)
            {
                dataContext.Products.Remove(products);
>>>>>>> origin/pol
                dataContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
<<<<<<< HEAD
            if (HttpContext.Session.GetString("userName") != null)
            {
=======
           
>>>>>>> origin/pol
                if (id == null)
                {
                    return NotFound();
                }

<<<<<<< HEAD
                var product = dataContext.Products
                    .FirstOrDefault(a => a.Id == id);
                if (product == null)
=======
                var autor = dataContext.Products
                    .FirstOrDefault(a => a.Id == id);
                if (autor == null)
>>>>>>> origin/pol
                {
                    return NotFound();
                }

<<<<<<< HEAD
                return View(product);
            }
            else
            {
                return Redirect("/");
            }
=======
                return View(autor);
            
              
            
>>>>>>> origin/pol
        }

        // POST: Autor/Edit/6
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


    }
}
