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

        // GET: CreditCard/Delete/id
        public IActionResult Delete(int? id)
        {
            //if (HttpContext.Session.GetString("userName") != null)
            //{
            if (id == null)
            {
                return NotFound();
            }

            var creditCard = dataContext.CreditCards
                .FirstOrDefault(a => a.Id == id);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }
        /*
        else
        {
            return Redirect("/");
        }
        */


        // POST: CreditCard/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var creditCard = dataContext.CreditCards.Find(id);
            if (creditCard != null)
            {
                dataContext.CreditCards.Remove(creditCard);
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
            var creditCard = dataContext.CreditCards
                .FirstOrDefault(a => a.Id == id);
            //.Find(id);
            if (creditCard == null)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            return View(creditCard);
            //}
        }

        // POST: CreditCard/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreditCard creditCard)
        {
            if (ModelState.IsValid)
            {
                var original = dataContext.CreditCards.Where(s => s.Id == creditCard.Id).FirstOrDefault();
                dataContext.Entry(original).CurrentValues.SetValues(creditCard);
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
                var creditCard = dataContext.CreditCards
                    .FirstOrDefault(a => a.Id == id);
                if (creditCard == null)
                {
                    return NotFound();
                }
                return View(creditCard);
        }
    }
}
