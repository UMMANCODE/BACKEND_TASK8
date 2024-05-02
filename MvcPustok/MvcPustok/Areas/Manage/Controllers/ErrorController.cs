using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcPustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }
    }
}

