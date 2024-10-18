using Microsoft.AspNetCore.Mvc;
using StarWars.Models;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace StarWars.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int[] starshipIds = [2, 3, 5,9,10,11,12,13,15,17];
            Random random = new Random();

            // Get a random index from the array
            int randomIndex = random.Next(0, starshipIds.Length);

            // api
            using HttpClient client = new HttpClient();

            // Send GET request and deserialize the response into the Starship object
            string url = "https://swapi.dev/api/starships/" + starshipIds[randomIndex];
            Starship starship = client.GetFromJsonAsync<Starship>(url).Result;


            return View(starship);
        }

        [HttpPost]
        public IActionResult SubmitForm(string selectedOption, IFormFile uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                // Handle file upload logic
                // For example, save the file to a directory, etc.
            }

            ViewBag.SelectedOption = selectedOption;
            return View("Index");

        }

        [HttpGet]
        public IActionResult Privacy()
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
