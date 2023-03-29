using Giovanni.Task2.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Giovanni.Task2.Infrastructure;

public class GiovanniDbContext: DbContext
{
    public GiovanniDbContext(DbContextOptions<GiovanniDbContext> options) : base(options)
    {

    }

    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Weather> Weathers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>()
            .HasOne(c => c.Country)
            .WithMany(c => c.Cities)
            .HasForeignKey(c => c.CountryId);
        modelBuilder.Entity<Weather>()
            .HasOne(w => w.City)
            .WithMany(c => c.Weathers)
            .HasForeignKey(w => w.CityId);
    }
}