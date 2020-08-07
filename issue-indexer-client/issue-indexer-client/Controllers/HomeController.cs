using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using issue_indexer_client.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace issue_indexer_client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string apiBaseURL;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            apiBaseURL = _configuration.GetValue<string>("WebAPIBaseURL");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel) {
            if (ModelState.IsValid) {
                try {
                    RequestSender client = new RequestSender(apiBaseURL);
                    var success = await client.Login(loginModel);
                    if (success) return RedirectToAction("Index", "Main", new { area = "Dashboard" });
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            return View(loginModel);
        }

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerModel) {
            if (ModelState.IsValid) {
                try {
                    RequestSender client = new RequestSender(apiBaseURL);
                    var success = await client.Register(registerModel);
                    if (success) return RedirectToAction("Login", "Home", new { area = "" });
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            return View(registerModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
