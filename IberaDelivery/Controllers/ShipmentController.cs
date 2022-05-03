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
    public class ShipmentController : Controller
    {
        private readonly iberiadbContext dataContext;

        public ShipmentController(iberiadbContext context)
        {
            dataContext = context;
        }


        public IActionResult Index()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
                {
                    return RedirectToAction("Index", "Home");
                }

                int id = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;

                var shipment = dataContext.Shipments
                .Where(a => a.UserId == id);
                return View(shipment.ToList());
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }


        }

        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ShipmentForm shipmentInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
                {
                    return RedirectToAction("Index", "Home");
                }
                User user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));

                if (ModelState.IsValid)
                {
                    Shipment shipment = new Shipment();
                    shipment.Address = shipmentInfo.Address;
                    shipment.Country = shipmentInfo.Country;
                    shipment.City = shipmentInfo.City;
                    shipment.PostalCode = shipmentInfo.PostalCode;
                    shipment.UserId = user.Id;
                    dataContext.Shipments.Add(shipment);
                    dataContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //ViewBag.missatge = autor.validarAutor().Missatge;
                    return View();
                }
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }

        public IActionResult Delete(int? id)
        {
            //if (HttpContext.Session.GetString("userName") != null)
            //{
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
                {
                    return RedirectToAction("Index", "Home");
                }
                if (id == null)
                {
                    return NotFound();
                }

                var shipmentInfo = dataContext.Shipments
                    .FirstOrDefault(a => a.Id == id);
                if (shipmentInfo == null)
                {
                    return NotFound();
                }

                return View(shipmentInfo);
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }
        /*
        else
        {
            return Redirect("/");
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
                {
                    return RedirectToAction("Index", "Home");
                }
                var shipmentInfo = dataContext.Shipments.Find(id);
                if (shipmentInfo != null)
                {
                    dataContext.Shipments.Remove(shipmentInfo);
                    dataContext.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }
        public IActionResult Edit(int? id)
        {
            //if (HttpContext.Session.GetString("userName") != null)
            //{
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
                {
                    return RedirectToAction("Index", "Home");
                }
                if (id == null)
                {
                    return NotFound();
                }
                var shipmentInfo = dataContext.Shipments
                    .FirstOrDefault(a => a.Id == id);
                //.Find(id);
                if (shipmentInfo == null)
                {
                    return NotFound();
                }

                ShipmentEditForm form = new ShipmentEditForm();
                form.Address = shipmentInfo.Address;
                form.City = shipmentInfo.City;
                form.Country = shipmentInfo.Country;
                form.PostalCode = shipmentInfo.PostalCode;
                form.Id = shipmentInfo.Id;
                ViewBag.Id = id;
                return View(form);
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ShipmentEditForm shipmentInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
                {
                    return RedirectToAction("Index", "Home");
                }
                if (ModelState.IsValid)
                {
                    Shipment regShipmentInfo = dataContext.Shipments.Where(s => s.Id == shipmentInfo.Id).FirstOrDefault();
                    regShipmentInfo.Address = shipmentInfo.Address;
                    regShipmentInfo.City = shipmentInfo.City;
                    regShipmentInfo.Country = shipmentInfo.Country;
                    regShipmentInfo.PostalCode = shipmentInfo.PostalCode;
                    dataContext.Shipments.Update(regShipmentInfo);
                    dataContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //ViewBag.missatge = category.validarCategory().msg;
                    return View();
                }
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }

    }
}
