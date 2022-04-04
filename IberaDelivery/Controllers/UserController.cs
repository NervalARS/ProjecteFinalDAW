using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IberaDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IberaDelivery.Controllers
{
    public class UserController : Controller
    {
        private readonly iberiadbContext dataContext;

        public UserController(iberiadbContext context)
        {
            dataContext = context;
        }

        // Return Home page.
        public ActionResult Index()
        {
            return View();
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
        public ActionResult Register(ViewRegister registerDetails)
        {
            //We check if the model state is valid or not. We have used DataAnnotation attributes.
            //If any form value fails the DataAnnotation validation the model state becomes invalid.
            if (ModelState.IsValid)
            {
                //If the model state is valid i.e. the form values passed the validation then we are storing the User's details in DB.
                User reglog = new User();

                //Save all details in RegitserUser object

                reglog.FirstName = registerDetails.FirstName;
                reglog.LastName = registerDetails.LastName;
                reglog.Email = registerDetails.Email;
                reglog.Password = registerDetails.Password;
                reglog.Rol = 3;


                //Calling the SaveDetails method which saves the details.
                dataContext.Users.Add(reglog);
                dataContext.SaveChanges();

                ViewBag.Message = "User Details Saved";
                return View("Register");
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
        public ActionResult Login(ViewLogin model)
        {
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {
                
                //Validating the user, whether the user is valid or not.
                var isValidUser = IsValidUser(model);

                //If user is valid & present in database, we are redirecting it to Welcome page.
                if (isValidUser != null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    //If the username and password combination is not present in DB then error message is shown.
                    ModelState.AddModelError("Failure", "Wrong Username and password combination !");
                    return View();
                }
            }
            else
            {
                //If model state is not valid, the model with error message is returned to the View.
                return View(model);
            }
        }

        //function to check if User is valid or not
        public User IsValidUser(ViewLogin model)
        {
            //Retireving the user details from DB based on username and password enetered by user.
            User user = dataContext.Users.Where(query => query.Email.Equals(model.Email) && query.Password.Equals(model.Password)).SingleOrDefault();
            //If user is present, then true is returned.
            if (user == null)
                return null;
            //If user is not present false is returned.
            else
                return user;
        }


        /*public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index");
        }*/
    }
}
