using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Newtonsoft.Json.Linq;

namespace MVC.Controllers;

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
    public IActionResult Homepage()
    {

        var token = HttpContext.Session.GetString("Token");

        if (token != null)
        {
            // Pass token to view
            ViewBag.Token = token;
            var tokenObject = JObject.Parse(token);
            var userId = tokenObject["user"]?["id"]?.ToString();
            ViewBag.UserId = userId;
            return View();
        }
        else
        {
            // Handle case where token is not available
            return RedirectToAction("Homepage", "home");
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
