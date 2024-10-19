using Microsoft.EntityFrameworkCore;
using StarWars.Models;

namespace StarWars.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Add DbSet for each table
        public DbSet<StarShipImage> StarShipImage { get; set; }

        // Optional: Override OnConfiguring if not using Dependency Injection
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Your_Connection_String");
        //}
    }

}