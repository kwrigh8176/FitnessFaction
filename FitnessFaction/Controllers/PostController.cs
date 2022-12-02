using Microsoft.AspNetCore.Mvc;

namespace FITNESS_FACTION_.NET_CORE_CONVERSIONS.Controllers
{
    public class PostController : Controller
    {
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePost(string username)
        {
            return View();
        }
    }
}
