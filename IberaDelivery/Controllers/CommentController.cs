using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace IberaDelivery.Controllers
{
    public class CommentController : Controller
    {
        private readonly iberiadbContext dataContext;

        public CommentController(iberiadbContext context)
        {
            dataContext = context;
        }

        // GET: Comment
        public async Task<IActionResult> Index()
        {
            var comments = dataContext.Comments
            .Include(u => u.User)
            .Include(p => p.Product)
            .AsNoTracking();
            return View(await comments.ToListAsync());

        }

        // Post: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(IFormCollection form) 
        {
            var id = int.Parse(form["id"]);
            var txt = form["com"];
            var pr = dataContext.Products.FirstOrDefault(a => a.Id == id);
            Comment comment = new Comment();
            comment.Contens = txt;
            comment.UserId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;
            comment.ProductId = pr.Id;
        
            dataContext.Add(comment);
            dataContext.SaveChanges();
            return RedirectToAction("Detail", "Product", new {id});
        }
        // GET: Comment/Delete/5
        public IActionResult Delete(int? id)
        {
            //if (HttpContext.Session.GetString("userName") != null)
            //{
            if (id == null)
            {
                return NotFound();
            }

            var comment = dataContext.Comments
                .FirstOrDefault(a => a.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var comment = dataContext.Comments.Find(id);
            if (comment != null)
            {
                dataContext.Comments.Remove(comment);
                dataContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var comment = dataContext.Comments
                .FirstOrDefault(a => a.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            return View(comment);
        }

        // POST: Comment/Edit/6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id", "Name")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var original = dataContext.Comments.Where(s => s.Id == comment.Id).FirstOrDefault();
                dataContext.Entry(original).CurrentValues.SetValues(comment);
                dataContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //ViewBag.missatge = category.validarCategory().msg;
                return View();
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
