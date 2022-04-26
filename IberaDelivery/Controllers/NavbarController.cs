using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace IberaDelivery.Controllers
{
    public class NavbarController : Controller
    {
        private readonly iberiadbContext dataContext;

        public NavbarController(iberiadbContext context)
        {
            dataContext = context;
        }

        [HttpPost]
        public IActionResult Search(IFormCollection form)
        {
            var products = dataContext.Products.ToList();
            ViewBag.Search = form["SearchField"];
            string searchField = form["SearchField"];
            int category = int.Parse(form["Category"]);
            if(searchField != null){
                if(category != 0){
                    products = dataContext.Products.Where(a => a.Name.Contains(searchField) && a.CategoryId.Equals(category)).Include(i => i.Images).Include(c => c.Category).Include(p => p.Provider).ToList();
                }else{
                    products = dataContext.Products.Where(a => a.Name.Contains(searchField)).Include(i => i.Images).Include(c => c.Category).Include(p => p.Provider).ToList();
                }
                return View(products);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
