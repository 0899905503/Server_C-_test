using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Newtonsoft.Json.Linq;

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

    public IActionResult GuestInfo()
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

    [HttpPost]
    public IActionResult SaveGuestInfo(string name, string address, string phone_number)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone_number))
        {
            // Set error message and return to GuestInfo view
            ViewBag.ErrorMessage = "Please fill in all required fields.";
            return View("GuestInfo");
        }

        // Redirect to Homepage with query parameters
        return RedirectToAction("Homepage", "Home", new { name = name, address = address, phone_number = phone_number });
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
