using Microsoft.EntityFrameworkCore;
using StarWars.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
namespace StarWars.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<StarShipImage> StarShipImage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}