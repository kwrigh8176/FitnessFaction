using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FitnessFaction.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult ForbiddenError()
        {
            return View();
        }
    }
}