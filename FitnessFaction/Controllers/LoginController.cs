using FitnessFaction.Database;
using FitnessFaction.Models;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Xml.Linq;

namespace FitnessFaction.Controllers
{
    public class LoginController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AzureRDBMS_Connection _azureRDBMS_Connection;

        public LoginController(IWebHostEnvironment webHostEnvironment, AzureRDBMS_Connection azureRDBMS_Connection)
        {
            _webHostEnvironment = webHostEnvironment;
            _azureRDBMS_Connection = azureRDBMS_Connection;
        }

        public ActionResult Login()
        {
            return View();
        }

        //if the credentials on the login page are submitted
        [HttpPost]
        public ActionResult Login(string username, string password)
        {

      
            string trimmedUsername = username.Trim();
            string trimmedPassword = password.Trim();

            //check if the credentials are valid
            if (MongoDatabaseConnection.userLoginCredentialsValid(trimmedUsername, trimmedPassword))
            {
                TempData["messages"] = "";



                //store the current username in the session
                HttpContext.Session.SetString("username", trimmedUsername);
                HttpContext.Session.SetString("globalOrFollow", "global");
                HttpContext.Session.SetString("feedType", "Fit");
                HttpContext.Session.SetString("pfp", _azureRDBMS_Connection.getProfilePicture(trimmedUsername));
                
                //redirct to the home feed
                return Redirect("/Home/" + trimmedUsername);
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

            return View(new SignUpViewModel());
        }

        [HttpPost]
        public ActionResult SignUp(SignUpViewModel signUpObj)
        {
            ViewData["UsernameStyle"] = "black";
            ViewData["EmailStyle"] = "black";


            string trimmedEmail = signUpObj.Email.Trim();
            string trimmedUsername = signUpObj.UserName.Trim();
            string trimmedPassword = signUpObj.Password.Trim();

            signUpObj.Email = trimmedEmail;
            signUpObj.UserName = trimmedUsername;
            signUpObj.Password = trimmedPassword;

            MailAddress temp;

            var valid = true;

            //checking for a valid email format
            try
            {
                temp = new MailAddress(trimmedEmail);
            }
            catch
            {

                valid = false;
            }


            string credentialVerification = MongoDatabaseConnection.userCredentialsValid(trimmedEmail, trimmedUsername);
            if (credentialVerification == "Credentials ok." && valid)
            {
                SignUpViewModel parsedViewModel = null;
                if (signUpObj.ProfilePicture == null)
                {
                    ViewData["profilePic"] = @"images/defaultpfp.jpg";
                }
                else
                {
                    parsedViewModel = uploadFile(signUpObj);
                    ViewData["profilePic"] = parsedViewModel.imageUrl;
                }

                

                //if valid log them in
                //(redirect to profile page for now)
                ViewBag.Message = "Credentials accepted!, Login!";
                MongoDatabaseConnection.postUserSignUpCredentials(parsedViewModel);

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

        private SignUpViewModel uploadFile(SignUpViewModel signUpObj)
        {

            if (signUpObj.ProfilePicture != null)
            {
                string folder = "images/";
                folder += Guid.NewGuid().ToString() + "_" + signUpObj.ProfilePicture.FileName;
                signUpObj.imageUrl = folder;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                signUpObj.ProfilePicture.CopyTo(new FileStream(serverFolder, FileMode.Create));



            }
            return signUpObj;
        }
    }
}