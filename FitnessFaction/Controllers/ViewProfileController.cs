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
            ViewData["currentUser"] = username;
            ViewData["visitingUser"] = HttpContext.Session.GetString("username");

            HttpContext.Session.SetString("route", "profile");
            HttpContext.Session.SetString("currentUser", username);

            ViewData["different"] = false;
            if (ViewData["visitingUser"].ToString() != username)
            {
                ViewData["different"] = true;
                if (_RDBMS_Connection.checkFollowing(username, ViewData["visitingUser"].ToString()))
                {
                    ViewData["following"] = true;
                }
                else
                {
                    ViewData["following"] = false;

                }
            }

            ViewData["profilePic"] = _RDBMS_Connection.getProfilePicture(username);
            int[] followingCounts = _RDBMS_Connection.getFollowCounts(username);

            ViewData["followedAccounts"] = followingCounts[0];
            ViewData["accountsFollowing"] = followingCounts[1];

            List <Posts> posts = _RDBMS_Connection.getProfilePosts(username);
            return View(posts);
        }

        public IActionResult Follow(string currentUser, string visitingUser)
        {
            if (currentUser != visitingUser)
            {
                _RDBMS_Connection.follow(currentUser, visitingUser);
            }
            
            return RedirectToAction("Profile", new { username = currentUser });
        }

        public IActionResult Unfollow(string currentUser, string visitingUser)
        {
            if (currentUser != visitingUser)
            {
                _RDBMS_Connection.unfollow(currentUser, visitingUser);
            }

            return RedirectToAction("Profile", new { username = currentUser });
        }
    }
}
