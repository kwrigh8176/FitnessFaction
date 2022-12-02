using Microsoft.AspNetCore.Mvc;

namespace FITNESS_FACTION_.NET_CORE_CONVERSIONS.Controllers
{
    public class ViewProfileController : Controller
    {
        public IActionResult Profile(string username)
        {
            //return the view of the user's profile
            if (HttpContext.Session.GetString("username") == username)
            {

            }
            //return external view of another person's profile
            else
            {

            }
            return View();
        }

    }
}
