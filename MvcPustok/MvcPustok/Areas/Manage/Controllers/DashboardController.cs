using System;
using Microsoft.AspNetCore.Mvc;
using MvcPustok.Data;

namespace MvcPustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class DashboardController:Controller
	{
	
		public IActionResult Index()
		{
			return View();
		}
	}
}
