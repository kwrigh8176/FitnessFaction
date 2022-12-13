using FitnessFaction;
using FitnessFaction.Database;
using Microsoft.AspNetCore.Mvc;

namespace FITNESS_FACTION_.NET_CORE_CONVERSIONS.Controllers
{
    public class ViewProfileController : Controller
    {
        private readonly AzureRDBMS_Connection _RDBMS_Connection;
        private readonly MongoDatabaseConnection _MongoConnection;
        private List<Posts> postList { get; set; }

        public ViewProfileController(AzureRDBMS_Connection azureRDBMS_Connection, MongoDatabaseConnection mongoConnection)
        {
            _RDBMS_Connection = azureRDBMS_Connection;
            _MongoConnection = mongoConnection;
        }
        public IActionResult Profile(string username)
        {
            //currentUser is the profile currently being visited
            ViewData["currentUser"] = username;

            //visitingUser is the user currently visiting the profile
            ViewData["visitingUser"] = HttpContext.Session.GetString("username");

            //set route just for the back button
            HttpContext.Session.SetString("route", "profile");
            HttpContext.Session.SetString("currentUser", username);

            //use different to decide if the follow button appears
            ViewData["different"] = false;
            if (ViewData["visitingUser"].ToString() != username)
            {
                ViewData["different"] = true;
                //check to see if the visiting user is following the current user (follow button will show as follow/unfollow depending on the case)
                if (_RDBMS_Connection.checkFollowing(username, ViewData["visitingUser"].ToString()))
                {
                    ViewData["following"] = true;
                }
                else
                {
                    ViewData["following"] = false;

                }
            }
            //retrieve the profile picture
            ViewData["profilePic"] = _RDBMS_Connection.getProfilePicture(username);

            //retrieve the following counts
            int[] followingCounts = _RDBMS_Connection.getFollowCounts(username);
            ViewData["followedAccounts"] = followingCounts[0];
            ViewData["accountsFollowing"] = followingCounts[1];

            //get all of the posts from the current profile
            List <Posts> posts = _RDBMS_Connection.getProfilePosts(username);
            return View(posts);
        }

        public IActionResult Follow(string currentUser, string visitingUser)
        {
            //If the visiting user is NOT the current user, trigger a follow event
            if (currentUser != visitingUser)
            {
                _RDBMS_Connection.follow(currentUser, visitingUser);
            }
            
            return RedirectToAction("Profile", new { username = currentUser });
        }

        public IActionResult Unfollow(string currentUser, string visitingUser)
        {
            //If the visiting user is NOT the current user, trigger an unfollow event
            if (currentUser != visitingUser)
            {
                _RDBMS_Connection.unfollow(currentUser, visitingUser);
            }

            return RedirectToAction("Profile", new { username = currentUser });
        }
    }
}
