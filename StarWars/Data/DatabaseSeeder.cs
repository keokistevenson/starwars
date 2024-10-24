using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StarWars.Models;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarWars.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.StarshipImage.AnyAsync())
                return;  

            using (HttpClient client = new HttpClient())
            {
                var response1 = await client.GetStringAsync("https://swapi.dev/api/starships/2/");
                var response2 = await client.GetStringAsync("https://swapi.dev/api/starships/3/");

                var starship1 = JObject.Parse(response1);
                var starship2 = JObject.Parse(response2);

                var starshipImage1 = new StarshipImage
                {
                    StarshipId = "2",
                    Name = starship1["name"].ToString(),
                    Image = CopyImageToLocal("01.png")
                };

                var starshipImage2 = new StarshipImage
                {
                    StarshipId = "3",
                    Name = starship2["name"].ToString(),
                    Image = CopyImageToLocal("02.png")
                };

                context.StarshipImage.AddRange(starshipImage1, starshipImage2);
                await context.SaveChangesAsync();
            }
        }

        private static string CopyImageToLocal(string fileName)
        {
            string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "SeedImages", fileName);
            string targetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            }

            File.Copy(sourcePath, targetPath, true);  
            return $"/images/{fileName}";  
        }
    }

}
