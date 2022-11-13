using FitnessFaction.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FitnessFaction.Controllers
{
    //Ensures that logged in user is currently authorized
    
    public class HomeController : Controller
    {

        public HomeController()
        {

        }

        [CustomAuth]
        //The main feed is directed through this view
        public ActionResult HomeFeed(string username)
        {
            //session user name is checked to prevent users from looking at different feeds
            if (HttpContext.Session.GetString("username") == username)
            {
                return View("HomeFeed");
            }
            else if (HttpContext.Session.GetString("username") != null)
                return new StatusCodeResult(401);
            else
                return new StatusCodeResult(404);
        }

        
    }
}