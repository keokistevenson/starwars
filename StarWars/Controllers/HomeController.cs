using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarWars.Data;
using StarWars.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // Install Newtonsoft.Json package
using System.Collections.Generic;
using System.IO;

namespace StarWars.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<SelectStarship> starships = await FetchStarships();

            var randomStarship = await GetStarships(starships[new Random().Next(starships.Count)].StarShipId);

            // Pass both the random starship and the list of starships to the view
            var model = new StarshipViewModel
            {
                RandomStarship = randomStarship,
                StarshipList = starships
            };

            return View(model);
        }

        private async Task<List<SelectStarship>> FetchStarships()
        {
            List<SelectStarship> starships = new List<SelectStarship>();

            using (HttpClient client = new HttpClient())
            {
                string url = "https://swapi.dev/api/starships";
                var response = await client.GetStringAsync(url);

                JObject jsonResponse = JObject.Parse(response);
                var results = jsonResponse["results"];

                foreach (var item in results)
                {
                    starships.Add(new SelectStarship
                    {
                        StarShipId = item["url"].ToString().Split('/')[^2],
                        Name = item["name"].ToString()
                    });
                }
            }
            return starships;
        }

        private async Task<Starship> GetStarships(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://swapi.dev/api/starships/{id}/";
                var response = await client.GetStringAsync(url);

                JObject jsonResponse = JObject.Parse(response);
                return new Starship
                {
                    Name = jsonResponse["name"].ToString(),
                    Model = jsonResponse["model"].ToString(),
                    Manufacturer = jsonResponse["manufacturer"].ToString(),
                    CostInCredits = jsonResponse["cost_in_credits"].ToString(),
                    Crew = jsonResponse["crew"].ToString(),
                    Passengers = jsonResponse["passengers"].ToString(),
                    StarshipClass = jsonResponse["starship_class"].ToString()
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm(string selectedOption, IFormFile uploadedFile)
        {
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string fileName = Path.GetFileName(uploadedFile.FileName);
            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(stream);
            }

            // Save starship data and image path to the database
            var starship = new StarShipImage
            {
                StarShipId = selectedOption,
                Name = await GetStarshipName(selectedOption),
                Image = $"/images/{fileName}"
            };

            _context.StarShipImage.Add(starship);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private async Task<string> GetStarshipName(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://swapi.dev/api/starships/{id}/";
                var response = await client.GetStringAsync(url);
                JObject jsonResponse = JObject.Parse(response);
                return jsonResponse["name"].ToString();
            }
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
