using FitnessFaction.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FitnessFaction.Controllers
{
    //Ensures that logged in user is currently authorized
    
    public class HomeController : Controller
    {
        private AzureRDBMS_Connection RDBMS_Connection = new AzureRDBMS_Connection();
        private List<Posts> postList { get; set;}

        public HomeController()
        { 

        }

        //The main feed is directed through this view
        public ActionResult HomeFeed(string username)
        {

            string globalOrFollow = HttpContext.Session.GetString("globalOrFollow");
            ViewData["globalOrFollow"] = globalOrFollow;

            ViewData["username"] = username;

            string feedType = HttpContext.Session.GetString("feedType");
            ViewData["feedType"] = feedType;
            //retrieve posts based on feedType
            if (globalOrFollow == "global")
            {
                postList = RDBMS_Connection.getGlobalPosts(feedType);

            }
            else
            {

            }

            //session user name is checked to prevent users from looking at different feeds
            if (HttpContext.Session.GetString("username") == username)
            {
                return View("HomeFeed",postList);
            }
            else
                return RedirectToAction("ForbiddenError","Error");
            
        }

        //Global posts and following posts are switched on the home page
        public ActionResult switchGlobalOrFollow()
        {
            if (HttpContext.Session.GetString("globalOrFollow") == "following")
                HttpContext.Session.SetString("globalOrFollow", "global");
            else
                HttpContext.Session.SetString("globalOrFollow", "following");
            return RedirectToAction("HomeFeed",new { username = HttpContext.Session.GetString("username") });


        }
        //Switch to diet feed posts
        public ActionResult switchDiet()
        {
            HttpContext.Session.SetString("feedType", "Diet");
            return RedirectToAction("HomeFeed", new { username = HttpContext.Session.GetString("username") });
        }

        //Switch to fitness feed posts
        public ActionResult switchFitness()
        {
           HttpContext.Session.SetString("feedType", "Fit");
            return RedirectToAction("HomeFeed", new { username = HttpContext.Session.GetString("username") });
        }

    }
}