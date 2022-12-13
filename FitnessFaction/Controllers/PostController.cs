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
            ViewData["username"] = username;

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

            //if there is no post title given
            if (String.IsNullOrEmpty(PostTitle))
            {
                ViewBag.Message = "Title needed\n";
                checksPassed = false;
            }

            //if there is no post text given
            if (String.IsNullOrEmpty(PostText))
            {
                ViewBag.Message = "Post text needed\n";
                checksPassed = false;
            }

            //if no tags were selected
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

            //get the current user logged in
            string userName = HttpContext.Session.GetString("username");

            if (checksPassed)
            {
                //enter the post into the database
                azureConnect.enterPost(PostTitle, PostText, feedType, Tags, userName, HttpContext.Session.GetString("pfp"));

                //tell the user that the post was posted
                TempData["messages"] = "<script>alert('Post succeeded!');</script>";

                //go back to the home page
                return RedirectToAction("HomeFeed", "Home", new { username = userName });
            }
            else
            {
                //if the checks didn't pass, stay on the create a post page
                return RedirectToAction("CreatePost");
            }


        }

        //controller method to view ONE singular post (as a detailed view)
        public ActionResult ViewPost(int id)
        {
            //retrieve variables for the post view
            ViewData["username"] = HttpContext.Session.GetString("username");
            ViewData["route"] = HttpContext.Session.GetString("route");
            ViewData["currentUser"] = HttpContext.Session.GetString("currentUser");

            //get the singular post via the id
            Posts post = azureConnect.getSinglePosts(id);
            List<Posts> posts = new List<Posts>() { post };

            //return the post to the view
            return View(posts);
        }


    }
}
