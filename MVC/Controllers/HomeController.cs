using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Models;
using Newtonsoft.Json.Linq;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Usercart()
        {
            return View();
        }

        public IActionResult Homepage()
        {
            var token = HttpContext.Session.GetString("Token");
            var role = HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(token))
            {
                // Pass token to view
                ViewBag.Token = token;

                var userJson = HttpContext.Session.GetString("User");
                if (!string.IsNullOrEmpty(userJson))
                {
                    var user = JObject.Parse(userJson);
                    var userId = user["id"]?.ToString();
                    ViewBag.UserId = userId;
                }

                if (role == "admin")
                {
                    return RedirectToAction("Admin", "Admin");
                }
                else
                {
                    // Process as a normal user
                    return View();
                }
            }
            else
            {
                // Allow guest users to access the homepage
                return View();
            }
        }


        public IActionResult Orderpage()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
