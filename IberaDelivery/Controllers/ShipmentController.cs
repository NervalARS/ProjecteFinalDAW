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
                    return View();
                }
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500", "Home");
            }

        }
        // GET: Shipment/Delete/5
        public IActionResult Delete(int? id)
        {
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
        // POST: Shipment/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var userId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;
                var shipmentInfo = dataContext.Shipments.Find(id);
                if (shipmentInfo != null)
                {
                    if (shipmentInfo.UserId == userId)
                    {
                        var Orders = dataContext.Orders.Where(a => a.ShipmentId == shipmentInfo.Id).ToList();
                        foreach (var order in Orders)
                        {
                            order.ShipmentId = null;
                            dataContext.Update(order);
                        }
                        dataContext.Shipments.Remove(shipmentInfo);
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("Error500", "Home");
                    }
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
