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

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    List<SelectStarship> starships = await FetchStarships();

        //    var randomStarshipImage = await _context.StarShipImage
        //    .OrderBy(r => EF.Functions.Random())
        //    .FirstOrDefaultAsync();

        //    var randomStarship = await GetStarships(starships[new Random().Next(starships.Count)].StarShipId);

        //    // Pass both the random starship and the list of starships to the view
        //    var model = new StarshipViewModel
        //    {
        //        RandomStarship = randomStarship,
        //        StarshipList = starships,
        //        StarshipImagePath = randomStarshipImage?.Image
        //    };

        //    return View(model);
        //}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var starShipImages = await _context.StarShipImage.ToListAsync();
            return View(starShipImages);
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
        public async Task<IActionResult> Create()
        {
            List<SelectStarship> availableStarships = await GetAvailableStarships();
            return View(availableStarships);
        }

        private async Task<List<SelectStarship>> GetAvailableStarships()
        {
            // Get all StarshipIds that are already in the database
            var existingStarshipIds = await _context.StarShipImage
                .Select(s => s.StarShipId)
                .ToListAsync();

            List<SelectStarship> allStarships = new List<SelectStarship>();

            using (HttpClient client = new HttpClient())
            {
                string url = "https://swapi.dev/api/starships";
                var response = await client.GetStringAsync(url);
                JObject jsonResponse = JObject.Parse(response);
                var results = jsonResponse["results"];

                foreach (var item in results)
                {
                    string starshipId = (string)item["url"].ToString().Split('/')[^2];

                    // Only add starships that are not in the database
                    if (!existingStarshipIds.Contains(starshipId))
                    {
                        allStarships.Add(new SelectStarship
                        {
                            StarShipId = starshipId,
                            Name = item["name"].ToString()
                        });
                    }
                }
            }

            return allStarships;
        }

        [HttpPost]
        public async Task<IActionResult> Create(StarShipImage model, IFormFile uploadedFile)
        {
            if (uploadedFile != null)
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

                model.Image = $"/images/{fileName}";
            }

            _context.StarShipImage.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var starship = await _context.StarShipImage.FindAsync(id);
            if (starship == null)
            {
                return NotFound();
            }
            return View(starship);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, IFormFile uploadedFile)
        {
            var starship = await _context.StarShipImage.FindAsync(id);
            if (starship == null)
            {
                return NotFound();
            }

            if (uploadedFile != null)
            {
                // Delete the old image if it exists
                string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", starship.Image.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Upload the new image
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

                // Update the image path in the database
                starship.Image = $"/images/{fileName}";
            }

            _context.Entry(starship).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Confirm delete (Get the entry to delete)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var starship = await _context.StarShipImage.FindAsync(id);
            if (starship == null)
            {
                return NotFound();
            }
            return View(starship);
        }

        // Handle the delete request (Delete operation)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var starship = await _context.StarShipImage.FindAsync(id);
            if (starship != null)
            {
                _context.StarShipImage.Remove(starship);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
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
