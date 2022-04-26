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
        public IActionResult Search(SearchModel model)
        {
            List<Product> products = new List<Product>();
            ViewBag.Search = model.SearchField;
            if(model.SearchField != null){
                if(model.Category != 0){
                    products = dataContext.Products.Where(a => a.Name.Contains(model.SearchField) && a.CategoryId.Equals(model.Category)).Include(i => i.Images).Include(c => c.Category).Include(p => p.Provider).ToList();
                }else{
                    products = dataContext.Products.Where(a => a.Name.Contains(model.SearchField)).Include(i => i.Images).Include(c => c.Category).Include(p => p.Provider).ToList();
                }
                return View(products);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
