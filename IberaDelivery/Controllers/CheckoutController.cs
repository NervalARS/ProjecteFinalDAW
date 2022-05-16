using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace IberaDelivery.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly iberiadbContext dataContext;
        public CheckoutController(iberiadbContext context)
        {
            dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            if (!checkUserExists() || checkUserIsClient())
            {
                return RedirectToAction("Index", "Home");
            }
            List<Product> ShoppingCart;
            ShoppingCart = new List<Product>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                ShoppingCart = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
                ViewBag.Products = ShoppingCart;
            }

            return View("ShoppingCart", ShoppingCart);
        }

        private void PopulateShipmentsDropDownList(object? selectedShipment = null)
        {
            var UserId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;
            var shipments = dataContext.Shipments.Where(a => a.UserId == UserId);
            ViewBag.ShipmentId = new SelectList(shipments.ToList(), "Id", "Address", selectedShipment);
        }
        private void PopulateCardsDropDownList(object? selectedCard = null)
        {
            var UserId = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Id;
            var cards = dataContext.CreditCards.Where(a => a.UserId == UserId);
            ViewBag.Cards = new SelectList(cards.ToList(), "Id", "Cardholder", selectedCard);
        }

        private void PopulateProductsList(object? selectedShipment = null)
        {
            List<Product> ShoppingCart;
            ShoppingCart = new List<Product>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                ShoppingCart = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
                ViewBag.Products = ShoppingCart;
            }
            else
            {
                ViewBag.Alert_EmptyCart = "Your shopping cart is empty!";
            }
        }
        private void CheckStock()
        {
            var ShoppingCart = new List<Product>();
            var ProductsWOstock = new List<Product>();
            ShoppingCart = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            foreach (var item in ShoppingCart)
            {
                var lnOrder = new LnOrder();
                lnOrder.Quantity = item.Stock;
                var product = dataContext.Products
               .FirstOrDefault(a => a.Id == item.Id);
                // SI NO HAY STOCK
                if (product.Stock < lnOrder.Quantity)
                {
                    ProductsWOstock.Add(product);
                }
            }
            ViewBag.ProductsWOstock = ProductsWOstock;
        }

        public IActionResult FixStock()
        {
            var ShoppingCart = new List<Product>();
            ShoppingCart = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            foreach (var item in ShoppingCart)
            {
                var product = dataContext.Products
               .FirstOrDefault(a => a.Id == item.Id);
                if (product.Stock < item.Stock)
                {
                    item.Stock = product.Stock;
                    item.Price = (item.Price + (item.Price * item.Iva / 100)) * item.Stock;
                }
            }
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(ShoppingCart));
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CheckoutDetails()
        {
            PopulateShipmentsDropDownList();
            PopulateCardsDropDownList();
            PopulateProductsList();
            CheckStock();
            ViewBag.User = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));
            return View("Checkout");
        }

        public async Task<IActionResult> Checkout(CheckoutForm model)
        {
            try
            {
                // Generar variables para utilizar mas tarde
                var orderOk = true;
                DateTime today = DateTime.Today;
                var ShoppingCart = new List<Product>();
                var ProductsWOstock = new List<Product>();

                ShoppingCart = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
                var orders = dataContext.Orders;
                var User = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));
                var order = new Order();
                // Rellenamos los datos de Order
                order.Date = today;
                order.UserId = User.Id;
                order.CreditCardId = model.CardId;
                order.ShipmentId = model.ShipmentId;
                // De momento le asigno un Import de 0, ya que necesito la Id que se le asignara al crearlo y el campo no admite valores nulos.
                order.Import = 0;
                dataContext.Add(order);
                dataContext.SaveChanges();
                CheckStock();
                if (ViewBag.ProductsWOstock.Count == 0)
                {
                    foreach (var item in ShoppingCart)
                    {
                        // Generamos una lnOrder y la rellenamos.
                        var lnOrder = new LnOrder();
                        lnOrder.NumOrder = order.Id;
                        lnOrder.RefProduct = item.Id;
                        // La Quantity de lnOrder sera el stock del carrito.
                        lnOrder.Quantity = item.Stock;
                        // El precio total es el Price + Iva.
                        lnOrder.TotalImport = item.Price + (item.Price * item.Iva / 100);
                        var product = dataContext.Products
                        .FirstOrDefault(a => a.Id == item.Id);
                        // Las comprobaciones ya se hicieron en CheckStock, solo falta efectuar los cambios
                        // Añadimos la LnOrder a la BDD
                        dataContext.Add(lnOrder);
                        //dataContext.SaveChanges();
                        // Actualizamos el Stock del producto en la BDD
                        product.Stock = product.Stock - lnOrder.Quantity;
                        dataContext.Update(product);
                        dataContext.SaveChanges();
                        // Actualizamos el importe del pedido
                        order.Import = order.Import + lnOrder.TotalImport;
                    }
                }
                else
                {
                    ViewBag.ProductsWOstock = ProductsWOstock;
                    return RedirectToAction("CheckoutDetails");
                }
                dataContext.Update(order);
                dataContext.SaveChanges();
                HttpContext.Session.Remove("Cart");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return RedirectToAction("Error500", "Home");
            }
        }
        public async Task<IActionResult> AddToCart(int? id, String? src)
        {
            List<Product> list;
            if (HttpContext.Session.GetString("Cart") == null)
            {
                list = new List<Product>();
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            else
            {
                list = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            }
            var product = buscarProducte(id);
            var oldStock = product.Stock;
            if (product != null)
            {
                if (list.FirstOrDefault(a => a.Id == id) != null)
                {
                    var pr = list.FirstOrDefault(a => a.Id == id);
                    if (product.Stock > pr.Stock)
                    {
                        pr.Stock = pr.Stock + 1;
                        pr.Price = (pr.Price + product.Price);
                        pr.Iva = pr.Iva;
                        list.Remove(list.FirstOrDefault(a => a.Id == id));
                        list.Add(pr);
                    }
                }
                else
                {
                    product.Stock = 1;
                    list.Add(product);
                }
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            if (product != null)
            {
                product.Stock = oldStock;
            }

            return RedirectToAction("Search", "Navbar", new { src, product.Category.Id });
        }

        public async Task<IActionResult> AddOne(int? id)
        {
            List<Product> list;
            if (HttpContext.Session.GetString("Cart") == null)
            {
                list = new List<Product>();
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            else
            {
                list = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            }
            var product = buscarProducte(id);
            var oldStock = product.Stock;
            if (product != null)
            {
                var pr = list.FirstOrDefault(a => a.Id == id);
                if (product.Stock > pr.Stock)
                {
                    pr.Stock = pr.Stock + 1;
                    pr.Price = (pr.Price + product.Price);
                    pr.Iva = pr.Iva;
                    list.Remove(list.FirstOrDefault(a => a.Id == id));
                    list.Add(pr);
                    HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
                }
            }
            if (product != null)
            {
                product.Stock = oldStock;
            }
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> RemoveOne(int? id)
        {
            List<Product> list;
            if (HttpContext.Session.GetString("Cart") == null)
            {
                list = new List<Product>();
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            else
            {
                list = JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("Cart"));
            }
            var product = buscarProducte(id);
            var oldStock = product.Stock;
            if (product != null)
            {
                if (list.FirstOrDefault(a => a.Id == id) != null)
                {
                    var pr = list.FirstOrDefault(a => a.Id == id);
                    pr.Stock = pr.Stock - 1;
                    pr.Price = (pr.Price - product.Price);
                    pr.Iva = pr.Iva;
                    list.Remove(list.FirstOrDefault(a => a.Id == id));
                    if (pr.Stock > 0)
                    {
                        list.Add(pr);
                    }
                }
                else
                {
                    product.Stock = 1;
                    list.Add(product);
                }
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(list));
            }
            if (product != null)
            {
                product.Stock = oldStock;
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return View("Index");
        }
        public Product buscarProducte(int? id)
        {
            var product = dataContext.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Provider)
                .Include(p => p.Comments)
                .FirstOrDefault(a => a.Id == id);
            if (product.Images != null)
            {
                foreach (var image in product.Images)
                {
                    image.Product = null;
                }
            }
            if (product.Comments != null)
            {
                foreach (var comment in product.Comments)
                {
                    comment.Product = null;
                    comment.User = null;
                }
            }
            product.Category.Products = null;
            product.Provider.Products = null;
            return product;
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