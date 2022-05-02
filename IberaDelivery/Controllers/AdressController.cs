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
    public class AdressController : Controller
    {
        private readonly iberiadbContext dataContext;

        public AdressController(iberiadbContext context)
        {
            dataContext = context;
        }

        // GET: Adress
        public IActionResult Index(int id)
        {
            var address = dataContext.Adresses
            .Where(a => a.UserId == id);
            return View(address.ToList());

        }

        

        // GET: Adress/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Adress/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Adress adress)
        {

            if (ModelState.IsValid)
            {
                dataContext.Add(adress);
                dataContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //ViewBag.missatge = autor.validarAutor().Missatge;
                return View();
            }
        }

        // GET: Adress/Delete/5
        public IActionResult Delete(int? id)
        {
            //if (HttpContext.Session.GetString("userName") != null)
            //{
            if (id == null)
            {
                return NotFound();
            }

            var adress = dataContext.Adresses
                .FirstOrDefault(a => a.Id == id);
            if (adress == null)
            {
                return NotFound();
            }

            return View(adress);
        }
        /*
        else
        {
            return Redirect("/");
        }
        */


        // POST: Adress/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var adress = dataContext.Adresses.Find(id);
            if (adress != null)
            {
                dataContext.Adresses.Remove(adress);
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
            var adress = dataContext.Adresses
                .FirstOrDefault(a => a.Id == id);
            //.Find(id);
            if (adress == null)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            return View(adress);
            //}
        }

        // POST: Adress/Edit/6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Adress adress)
        {
            if (ModelState.IsValid)
            {
                var original = dataContext.Adresses.Where(s => s.Id == adress.Id).FirstOrDefault();
                dataContext.Entry(original).CurrentValues.SetValues(adress);
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
                var adress = dataContext.Adresses
                    .FirstOrDefault(a => a.Id == id);
                if (adress == null)
                {
                    return NotFound();
                }
                return View(adress);
        }
    }
}
