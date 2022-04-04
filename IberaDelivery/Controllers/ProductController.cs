using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pt1_mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly iberiadbContext dataContext;

        public ProductController(iberiadbContext context)
        {
            dataContext = context;
        }

        // GET: Autor
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
            .Where(p => p.Name.Contains(Cadena)); //|| a.Cognoms.Contains(Cadena));
            ViewBag.missatge = "Filtrat per: " + Cadena;

            return View(products.ToList());

        }

        private void PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            var categories = dataContext.Categories;
            ViewBag.CategoryId = new SelectList(categories.ToList(), "Id", "Name", selectedCategory);
        }

        private void PopulateProvidersDropDownList(object selectedProvider = null)
        {
            var providers = dataContext.Users;
            ViewBag.ProviderId = new SelectList(providers.ToList(), "Id", "FullName", selectedProvider);
        }


        // GET: Autor/Create
        public IActionResult Create()
        {
           PopulateCategoriesDropDownList();
           PopulateProvidersDropDownList();
                return View();
            
            
        }

        // POST: Autor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,CategoryId,ProviderId,Stock,Price,Iva")] Product product)
        {

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


        }

        // GET: Autor/Delete/5
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

        // POST: Autor/Delete/5
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

        public IActionResult Edit(int? id)
        {
           
                if (id == null)
                {
                    return NotFound();
                }

                var autor = dataContext.Products
                    .FirstOrDefault(a => a.Id == id);
                if (autor == null)
                {
                    return NotFound();
                }

                return View(autor);
            
              
            
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
