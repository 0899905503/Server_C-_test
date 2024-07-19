using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Models;
using Newtonsoft.Json.Linq;

namespace MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly HttpClient _httpClient;

        public AuthController(HttpClient httpClient, ILogger<AuthController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var loginData = new
                {
                    Username = username,
                    Password = password
                };

                var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://localhost:5014/api/Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JObject.Parse(jsonResponse);

                    var token = responseObject["token"]?.ToString();
                    var user = responseObject["user"]?.ToString();
                    var role = responseObject["user"]?["role"]?.ToString();

                    // Save the token, user and role in session
                    HttpContext.Session.SetString("Token", token);
                    HttpContext.Session.SetString("User", user);
                    HttpContext.Session.SetString("Role", role);

                    // Redirect based on role
                    if (role == "admin")
                    {
                        return RedirectToAction("Admin", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Homepage", "Home");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid username or password";
                    return View("Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Login");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
