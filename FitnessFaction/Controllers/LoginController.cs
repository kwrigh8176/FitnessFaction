using FitnessFaction.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace FitnessFaction.Controllers
{
    public class LoginController : Controller
    {

        public ActionResult Login()
        {
            return View();
        }

        //if the credentials on the login page are submitted
        [HttpPost]
        public ActionResult Login(string username, string password)
        {

            //check if the credentials are valid
            if (userDatabase.userLoginCredentialsValid(username, password))
            {

                //store the current username in the session
                HttpContext.Session.SetString("username", username);

                //redirct to the home feed
                return Redirect("/Home/" + username);
            }
            else
            {
                ViewBag.Message = "Credentials do not exist!";
                return View();
            }

        }


        public ActionResult SignUp()
        {
            return View(new SignUpModel());
        }

        [HttpPost]
        public ActionResult SignUp(string email, string username, string password)
        {
            MailAddress temp;

            var valid = true;

            //checking for a valid email format
            try
            {
                temp = new MailAddress(email);
            }
            catch
            {
              
               valid = false;
            }

            if (userDatabase.userCredentialsValid(email, username) && valid)
            {
                //if valid log them in
                //(redirect to profile page for now)
                ViewBag.Message = "Credentials accepted!, Login!";
                userDatabase.postUserSignUpCredentials(email, username, password);

                return View("Login");
            }
            else
            {
                ViewBag.Message = "Credentials not accepted!";
                return View();
            }



           
        }
    }
}