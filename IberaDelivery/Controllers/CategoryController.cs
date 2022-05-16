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
    public class CategoryController : Controller
    {
        private readonly iberiadbContext dataContext;

        public CategoryController(iberiadbContext context)
        {
            dataContext = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            if (checkUserIsAdmin())
            {
                var categories = dataContext.Categories
                .Include(p => p.Products)
                .AsNoTracking();
                return View(await categories.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        [HttpPost]
        public IActionResult Index(String Cadena)
        {
            if (checkUserIsAdmin())
            {
                var categories = dataContext.Categories
                .Where(a => a.Name.Contains(Cadena));
                ViewBag.missatge = "Filtrat per: " + Cadena;
                return View(categories.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        private void PopulateProductesDropDownList(object? selectedProduct = null)
        {
            var productes = dataContext.Products;
            ViewBag.ProducteId = new SelectList(productes.ToList(), "Id", "Name", selectedProduct);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            if (checkUserIsAdmin())
            {
                PopulateProductesDropDownList();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name")] Category category)
        {
            if (checkUserIsAdmin())
            {
                if (ModelState.IsValid)
                {
                    dataContext.Add(category);
                    dataContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //ViewBag.missatge = autor.validarAutor().Missatge;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: Category/Delete/5
        public IActionResult Delete(int? id)
        {
            if (checkUserIsAdmin())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var category = dataContext.Categories
                    .FirstOrDefault(a => a.Id == id);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // POST: Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (checkUserIsAdmin())
            {
                var category = dataContext.Categories.Find(id);
                if (category != null)
                {
                    dataContext.Categories.Remove(category);
                    dataContext.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }
        public IActionResult Edit(int? id)
        {
            if (checkUserIsAdmin())
            {
                if (id == null)
                {
                    return NotFound();
                }
                var category = dataContext.Categories
                    .FirstOrDefault(a => a.Id == id);
                //.Find(id);
                if (category == null)
                {
                    return NotFound();
                }
                ViewBag.Id = id;
                return View(category);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // POST: Category/Edit/6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id", "Name")] Category category)
        {
            if (checkUserIsAdmin())
            {
                if (ModelState.IsValid)
                {
                    var original = dataContext.Categories.Where(s => s.Id == category.Id).FirstOrDefault();
                    dataContext.Entry(original).CurrentValues.SetValues(category);
                    dataContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //ViewBag.missatge = category.validarCategory().msg;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }


        public IActionResult Details(int? id)
        {
            if (checkUserIsAdmin())
            {
                if (id == null)
                {
                    return NotFound();
                }
                var categoria = dataContext.Categories
                    .FirstOrDefault(a => a.Id == id);
                if (categoria == null)
                {
                    return NotFound();
                }
                return View(categoria);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
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
            if (checkUserExists())
            {
                // If (rol == 3) return true
                if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 3)
                {
                    return true;
                }
            }
            // Else return false;
            return false;
        }
        public bool checkUserIsProveidor()
        {
            if (checkUserExists())
            {
                // If (rol == 2) return true
                if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 2)
                {
                    return true;
                }
            }
            // Else return false;
            return false;
        }
        public bool checkUserIsAdmin()
        {
            if (checkUserExists())
            {
                // If (rol == 1) return true
                if (JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol == 1)
                {
                    return true;
                }
            }
            // Else return false;
            return false;
        }
    }
}
