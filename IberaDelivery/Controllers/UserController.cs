using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using IberaDelivery.Models;
using IberaDelivery.Services;

namespace IberaDelivery.Controllers
{
    public class UserController : Controller
    {
        private readonly iberiadbContext dataContext;

        public UserController(iberiadbContext context)
        {
            dataContext = context;
        }

        private string GenerateToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Base64UrlTextEncoder.Encode(time.Concat(key).ToArray());
            return token;
        }
        private void showUsers()
        {
            if (!(string.IsNullOrEmpty(HttpContext.Session.GetString("user"))))
            {
                var user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));
                ViewBag.userName = user.FirstName + " " + user.LastName;
            }
        }
        // Return Home page.
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            var users = dataContext.Users;
            return View(users.ToList());
        }

        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ViewUserCreate user)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {

                try
                {

                    User reglog = new User();

                    //Save all details in RegitserUser object

                    reglog.FirstName = user.FirstName;
                    reglog.LastName = user.LastName;
                    reglog.Email = user.Email;
                    reglog.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    reglog.Rol = user.Rol;
                    reglog.Activate = true;

                    dataContext.Users.Add(reglog);
                    dataContext.SaveChanges();
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error: " + e;
                    return RedirectToAction("Error500", "Home");
                }
            }
            else
            {
                //ViewBag.missatge = autor.validarAutor().Missatge;
                return View();
            }


        }

        public IActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {

                var user = dataContext.Users
                .FirstOrDefault(a => a.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                ViewUserEdit editUser = new ViewUserEdit();

                editUser.Id = id;
                editUser.FirstName = user.FirstName;
                editUser.LastName = user.LastName;
                editUser.Email = user.Email;
                editUser.Rol = user.Rol;
                editUser.Password = user.Password;
                return View(editUser);

            }
            catch (Exception e)
            {
                ViewBag.Message = "Error: " + e;
                return RedirectToAction("Error500", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ViewUserEdit user)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {

                if (ModelState.IsValid)
                {
                    User reglog = dataContext.Users.Find(user.Id);

                    //Update all details in object
                    if (reglog.FirstName != user.FirstName)
                    {
                        reglog.FirstName = user.FirstName;
                    }
                    if (reglog.LastName != user.LastName)
                    {
                        reglog.LastName = user.LastName;
                    }
                    if (reglog.Email != user.Email)
                    {
                        reglog.Email = user.Email;
                    }
                    if (reglog.Password != user.Password)
                    {
                        reglog.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    }
                    if (reglog.Rol != user.Rol)
                    {
                        reglog.Rol = user.Rol;
                    }

                    dataContext.Users.Update(reglog);
                    dataContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //ViewBag.missatge = autor.validarAutor().Missatge;
                    return View(user);
                }

            }
            catch (Exception e)
            {
                ViewBag.Message = "Error: " + e;
                return RedirectToAction("Error500", "Home");
            }

        }

        public IActionResult Delete(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = dataContext.Users
                .FirstOrDefault(a => a.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var user = dataContext.Users.Find(id);
                user.Activate = false;
                dataContext.Users.Update(user);
                dataContext.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                ViewBag.Message = "Error: " + e;
                return RedirectToAction("Error500", "Home");
            }
        }

        public IActionResult Enable(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")) || JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user")).Rol != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var user = dataContext.Users.Find(id);
                user.Activate = true;
                dataContext.Users.Update(user);
                dataContext.SaveChanges();
                return RedirectToAction("Index", "User");

            }
            catch (Exception e)
            {
                ViewBag.Message = "Error: " + e;
                return RedirectToAction("Error500", "Home");
            }
        }

        //Return Register view
        public ActionResult Register()
        {
            return View();
        }

        //The form's data in Register view is posted to this method. 
        //We have binded the Register View with Register ViewModel, so we can accept object of Register class as parameter.
        //This object contains all the values entered in the form by the user.
        [HttpPost]
        public async Task<IActionResult> Register(ViewUserRegister registerDetails)
        {
            //We check if the model state is valid or not. We have used DataAnnotation attributes.
            //If any form value fails the DataAnnotation validation the model state becomes invalid.
            if (ModelState.IsValid)
            {
                try
                {
                    if (dataContext.Users.Any(x => x.Email.Equals(registerDetails.Email)))
                    {
                        ViewBag.Message = "Email is already in use";
                        return View("Register");
                    }
                    else
                    {
                        //If the model state is valid i.e. the form values passed the validation then we are storing the User's details in DB.
                        User reglog = new User();

                        //Save all details in RegitserUser object

                        reglog.FirstName = registerDetails.FirstName;
                        reglog.LastName = registerDetails.LastName;
                        reglog.Email = registerDetails.Email;
                        reglog.Password = BCrypt.Net.BCrypt.HashPassword(registerDetails.Password);
                        reglog.Rol = 3;
                        //var sender = new EmailSender(options);

                        string token = GenerateToken();

                        reglog.Token = token;

                        //Calling the SaveDetails method which saves the details.
                        dataContext.Users.Add(reglog);
                        dataContext.SaveChanges();

                        EmailSender sender = new EmailSender();

                        sender.SendActivationEmail(reglog.Email, token);

                        return RedirectToAction("Index", "Home");
                    }

                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error: " + e;
                    return RedirectToAction("Error500", "Home");
                }
            }
            else
            {

                //If the validation fails, we are returning the model object with errors to the view, which will display the error messages.
                return View("Register", registerDetails);
            }
        }


        public ActionResult Login()
        {
            return View();
        }

        //The login form is posted to this method.
        [HttpPost]
        public ActionResult Login(ViewUserLogin model)
        {
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {
                try
                {
                    //Validating the user, whether the user is valid or not.
                    var isValidUser = IsValidUser(model);

                    //If user is valid & present in database, we are redirecting it to Welcome page.
                    if (isValidUser != null)
                    {
                        if (isValidUser.Activate)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("Failure", "Account no activated");
                            ViewBag.Message = "You lost confirmation email? Resend.";
                            return View();
                        }
                    }
                    else
                    {
                        //If the username and password combination is not present in DB then error message is shown.
                        ModelState.AddModelError("Failure", "Wrong Username and password combination !");
                        return View();
                    }

                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error: " + e;
                    return RedirectToAction("Error500", "Home");
                }
            }
            else
            {
                //If model state is not valid, the model with error message is returned to the View.
                return View(model);
            }
        }

        //function to check if User is valid or not
        public User IsValidUser(ViewUserLogin model)
        {
            //Retireving the user details from DB based on username and password enetered by user.
            User user = dataContext.Users.Where(query => query.Email.Equals(model.Email)).SingleOrDefault();
            //If user is present, then true is returned.
            if (user == null)
            {
                return null;
                //If user is not present false is returned.
            }
            else
            {
                if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    createSession(user);
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public void createSession(User user)
        {
            HttpContext.Session.SetString("user", JsonSerializer.Serialize(user));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Managment(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = dataContext.Users
            .FirstOrDefault(a => a.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Managment(User user)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                User reglog = dataContext.Users.Find(user.Id);

                //Update all details in object
                if (reglog.FirstName != user.FirstName)
                {
                    reglog.FirstName = user.FirstName;
                }
                if (reglog.LastName != user.LastName)
                {
                    reglog.LastName = user.LastName;
                }

                dataContext.Users.Update(reglog);
                dataContext.SaveChanges();
                createSession(reglog);
                ViewBag.SaveMessage = "Saving changes.";
                return View(reglog);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Error: " + e;
                return RedirectToAction("Error500", "Home");
            }


        }

        public IActionResult NewPassword(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewUserNewPassword model = new ViewUserNewPassword();
            model.Id = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewPassword(ViewUserNewPassword model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    User user = dataContext.Users.Find(model.Id);

                    if (BCrypt.Net.BCrypt.Verify(model.OrgPassword, user.Password))
                    {

                        user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                        dataContext.Users.Update(user);
                        dataContext.SaveChanges();
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        ViewBag.Message = "The old password is incorrect";
                        return RedirectToAction("Error500", "Home");
                    }

                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error: " + e;
                    return RedirectToAction("Error500", "Home");
                }

                //Update all details in object

            }
            else
            {
                //ViewBag.missatge = autor.validarAutor().Missatge;
                return View();
            }


        }

        public IActionResult NewEmail(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return RedirectToAction("Index", "Home");
            }

            User model = new User();
            model.Id = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewEmail(User model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {

                    User user = dataContext.Users.Find(model.Id);

                    if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        user.Email = model.Email;
                        dataContext.Users.Update(user);
                        dataContext.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Message = "The old password is incorrect";
                        return View();
                    }

                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error: " + e;
                    return RedirectToAction("Error500", "Home");
                }
                //Update all details in object

            }
            else
            {
                //ViewBag.missatge = autor.validarAutor().Missatge;
                return View();
            }


        }

        public IActionResult ActivateAccount()
        {
            string token = HttpContext.Request.Query["token"].ToString(); ;

            if (token != null)
            {
                try
                {
                    User user = dataContext.Users.Where(a => a.Token.Equals(token)).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Token == token)
                        {
                            byte[] data = Base64UrlTextEncoder.Decode(token);
                            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                            if (when > DateTime.UtcNow.AddHours(-24))
                            {
                                user.Activate = true;
                                user.Token = null;

                                dataContext.Users.Update(user);
                                dataContext.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error: " + e;
                    return RedirectToAction("Error500", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ResendEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResendEmail(User model)
        {
            try
            {
                User user = dataContext.Users.Where(a => a.Email.Equals(model.Email)).FirstOrDefault();

                string token = GenerateToken();
                user.Token = token;
                dataContext.Users.Update(user);
                dataContext.SaveChanges();

                EmailSender sender = new EmailSender();
                sender.SendActivationEmail(user.Email, token);

                return RedirectToAction("Login", "User");

            }
            catch (Exception e)
            {
                ViewBag.Message = "Error: " + e;
                return RedirectToAction("Error500", "Home");
            }
        }

        public IActionResult ResetPasswordEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPasswordEmail(User model)
        {
            try
            {
                User user = dataContext.Users.Where(a => a.Email.Equals(model.Email)).FirstOrDefault();

                string token = GenerateToken();
                user.Token = token;
                dataContext.Users.Update(user);
                dataContext.SaveChanges();

                EmailSender sender = new EmailSender();
                sender.ResetPasswordEmail(user.Email, token);

                return RedirectToAction("Login", "User");

            }
            catch (Exception e)
            {
                ViewBag.Message = "Error: " + e;
                return RedirectToAction("Error500", "Home");
            }
        }

        public IActionResult ResetPassword()
        {
            string token = HttpContext.Request.Query["token"].ToString(); ;

            if (token != null)
            {
                try
                {
                    User user = dataContext.Users.Where(a => a.Token.Equals(token)).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Token == token)
                        {
                            byte[] data = Base64UrlTextEncoder.Decode(token);
                            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                            if (when > DateTime.UtcNow.AddHours(-24))
                            {
                                user.Token = null;

                                dataContext.Users.Update(user);
                                dataContext.SaveChanges();

                                ViewUserResetPassword model = new ViewUserResetPassword();
                                model.Id = user.Id;
                                return View(model);
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error: " + e;
                    return RedirectToAction("Error500", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ResetPassword(ViewUserResetPassword model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    User user = dataContext.Users.Find(model.Id);

                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    dataContext.Users.Update(user);
                    dataContext.SaveChanges();
                    return RedirectToAction("Login", "User");
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error: " + e;
                    return RedirectToAction("Error500", "Home");
                }
            }
            else
            {
                return View(model);
            }
        }
    }
}
