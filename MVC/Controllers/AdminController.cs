using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Models;

namespace MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly HttpClient _httpClient;

        public AdminController(HttpClient httpClient, ILogger<AdminController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }


        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Updatetaste()
        {
            return View();
        }
        public IActionResult Tastedetails()
        {
            return View();
        }
        public IActionResult Createtaste()
        {
            return View();
        }
    }

}
