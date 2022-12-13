using FitnessFaction.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace FitnessFaction.Controllers
{
    //Ensures that logged in user is currently authorized
    
    public class HomeController : Controller
    {
        private readonly AzureRDBMS_Connection _RDBMS_Connection;
        private List<Posts> postList { get; set;}

        public HomeController(AzureRDBMS_Connection azureRDBMS_Connection)
        { 
            _RDBMS_Connection = azureRDBMS_Connection;
        }

        //The main feed is directed through this view
        public ActionResult HomeFeed(string username)
        {

            //for filtering between global and following posts
            var globalOrFollow = HttpContext.Session.GetString("globalOrFollow");
            ViewData["globalOrFollow"] = globalOrFollow;

            //keep track of the username for pages contents
            ViewData["username"] = username;

            //check if the feed is the fitness or diet feed
            var feedType = HttpContext.Session.GetString("feedType");
            ViewData["feedType"] = HttpContext.Session.GetString("feedType");

            //retrieve the profile picture of the current user
            ViewData["profilePic"] = HttpContext.Session.GetString("pfp");

            //simple setting for back buttons
            HttpContext.Session.SetString("route", "home");

            //retrieve posts based on feedType
            if (globalOrFollow == "global")
            {
                postList = _RDBMS_Connection.getGlobalPosts(feedType);
               

            }
            else
            {
                postList = _RDBMS_Connection.getFollowingPosts(feedType, HttpContext.Session.GetString("username"));
            }

            //set the list to empty if there are not any posts retrieved
            if (postList == null)
            {
                postList = new List<Posts>();
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