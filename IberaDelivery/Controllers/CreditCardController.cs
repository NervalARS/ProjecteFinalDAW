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
    public class CreditCardController : Controller
    {
        private readonly iberiadbContext dataContext;

        public CreditCardController(iberiadbContext context)
        {
            dataContext = context;
        }

        // GET: CreditCard
        public async Task<IActionResult> Index()
        {
            var credCard = dataContext.CreditCards
            .AsNoTracking();
            return View(await credCard.ToListAsync());

        }

        // GET: CreditCard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CreditCard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreditCard creditCard)
        {

            if (ModelState.IsValid)
            {
                dataContext.Add(creditCard);
                dataContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //ViewBag.missatge = autor.validarAutor().Missatge;
                return View();
            }
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int? id)
        {
            //if (HttpContext.Session.GetString("userName") != null)
            //{
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
        /*
        else
        {
            return Redirect("/");
        }
        */


        // POST: Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var category = dataContext.Categories.Find(id);
            if (category != null)
            {
                dataContext.Categories.Remove(category);
                dataContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            //if (HttpContext.Session.GetString("userName") != null)
            //{
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
            //}
        }

        // POST: Category/Edit/6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id", "Name")] Category category)
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


        public IActionResult Details(int? id)
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
    }
}
