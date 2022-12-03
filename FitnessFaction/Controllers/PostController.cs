using FitnessFaction;
using FitnessFaction.Database;
using FitnessFaction.Models;
using Microsoft.AspNetCore.Mvc;

namespace FITNESS_FACTION_.NET_CORE_CONVERSIONS.Controllers
{
    public class PostController : Controller
    {
        AzureRDBMS_Connection azureConnect = new AzureRDBMS_Connection();
        public ActionResult CreatePost(string username)
        {
            //pull the tags via database
            List<Tags> getTags = azureConnect.pullTags();
            ViewData["tags"] = getTags;
            ViewData["postErrors"] = "";

            //session user name is checked to prevent users from looking at different feeds
            if (HttpContext.Session.GetString("username") == username)
                return View();
            else
                return RedirectToAction("ForbiddenError", "Error");
        }
        
        //pass fields when submit button is hit
        [HttpPost]
        public ActionResult CreatePost(string PostTitle, string PostText, bool dietPost, bool fitnessPost, string Tags)
        {
            bool checksPassed = true;
            TempData["messages"] = "";
            string feedType = "";

            if (String.IsNullOrEmpty(PostTitle))
            {
                ViewBag.Message = "Title needed\n";
                checksPassed = false;
            }

            if (String.IsNullOrEmpty(PostText))
            {
                ViewBag.Message = "Post text needed\n";
                checksPassed = false;
            }

            if (!dietPost && !fitnessPost)
            {
                ViewBag.Message = "Tags needed";
                checksPassed = false;
            }
            else if (dietPost && !fitnessPost)
            {
                feedType = "Diet";
            }
            else
            {
                feedType = "Fit";
            }

            string userName = HttpContext.Session.GetString("username");

            if (checksPassed)
            {
                azureConnect.enterPost( PostTitle,  PostText, feedType, Tags, userName, HttpContext.Session.GetString("pfp"));
                TempData["messages"] = "<script>alert('Post succeeded!');</script>";
                return RedirectToAction("HomeFeed","Home", new {username = userName });
            }
            else
            {
                return RedirectToAction("CreatePost");
            }

            
        }
    }
}
