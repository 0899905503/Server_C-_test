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
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    // Deserialize tokenResponse if needed
                    // var token = JsonSerializer.Deserialize<TokenResponse>(tokenResponse);
                    return RedirectToAction("Homepage", "home"); // Redirect to success view or return token
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid username or password";
                    return View("Login"); // Redirect back to login view with error message
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Login"); // Redirect back to login view with error message
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
