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
            if (MongoDatabaseConnection.userLoginCredentialsValid(username, password))
            {

                //store the current username in the session
                HttpContext.Session.SetString("username", username);
                HttpContext.Session.SetString("globalOrFollow", "global");
                HttpContext.Session.SetString("feedType", "Fit");
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
            ViewData["UsernameStyle"] = "black";
            ViewData["EmailStyle"] = "black";

            return View(new SignUpModel());
        }

        [HttpPost]
        public ActionResult SignUp(string email, string username, string password)
        {
            ViewData["UsernameStyle"] = "black";
            ViewData["EmailStyle"] = "black";

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

            string credentialVerification = MongoDatabaseConnection.userCredentialsValid(email, username);
            if (credentialVerification == "Credentials ok." && valid)
            {
                //if valid log them in
                //(redirect to profile page for now)
                ViewBag.Message = "Credentials accepted!, Login!";
                MongoDatabaseConnection.postUserSignUpCredentials(email, username, password);

                return View("Login");
            }
            else
            {
                if (credentialVerification == "Email and username taken.")
                {
                    ViewData["UsernameStyle"] = "red";
                    ViewData["EmailStyle"] = "red";
                }
                else if (credentialVerification == "Username taken")
                {
                    ViewData["UsernameStyle"] = "red";
                }
                else
                {
                    ViewData["EmailStyle"] = "red";
                }
                ViewBag.Message = "Credentials not accepted!";
                return View();
            }



           
        }
    }
}