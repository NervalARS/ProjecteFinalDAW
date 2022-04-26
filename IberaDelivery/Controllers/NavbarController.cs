using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using IberaDelivery.Models;
using IberaDelivery.Services;

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
                    products = dataContext.Products.Where(a => a.Name.Contains(model.SearchField) && a.CategoryId.Equals(model.Category)).ToList();
                }else{
                    products = dataContext.Products.Where(a => a.Name.Contains(model.SearchField)).ToList();
                }
                return View(products);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
