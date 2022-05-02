using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
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

namespace IberaDelivery.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly iberiadbContext dataContext;
    
    public HomeController(ILogger<HomeController> logger, iberiadbContext context)
    {
        _logger = logger;
        dataContext = context;
    }

    public IActionResult Index()
    {
        try{
            var products = dataContext.Products
            .OrderBy(a => a.Id)
            .Include(c => c.Category)
            .Include(p => p.Provider)
            .Include(i => i.Images);

            return View(products.ToList());
        }catch(Exception e){
            return RedirectToAction("Error500", "Home")
        }
    }

    public IActionResult Error500()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
