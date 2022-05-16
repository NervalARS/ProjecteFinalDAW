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
        public IActionResult Create(CreditCardForm creditCardForm)
        {

            if (ModelState.IsValid)
            {
                CreditCard creditCard = new CreditCard();

                creditCard.UserId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;
                creditCard.Cardholder = creditCardForm.Cardholder;
                //creditCard.CardNumber = creditCardForm.Number; Descomentar esto mas tarde xdddd
                creditCard.CardNumber = creditCardForm.CardNumber;

                dataContext.CreditCards.Add(creditCard);
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
            CreditCardEditForm CreditCardEditForm = new CreditCardEditForm();
            CreditCardEditForm.Id = creditCard.Id;
            CreditCardEditForm.Cardholder = creditCard.Cardholder;
            CreditCardEditForm.CardNumber = creditCard.CardNumber;
            //.Find(id);
            if (creditCard == null)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            return View(CreditCardEditForm);
            //}
        }

        // POST: CreditCard/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreditCardEditForm creditCard)
        {
            if (ModelState.IsValid)
            {
                var original = dataContext.CreditCards.Where(s => s.Id == creditCard.Id).FirstOrDefault();
                original.Cardholder = creditCard.Cardholder;
                original.CardNumber = creditCard.CardNumber;
                dataContext.Update(original);
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
