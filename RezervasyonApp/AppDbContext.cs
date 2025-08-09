using Microsoft.EntityFrameworkCore;
using RezervasyonApp.Models;

namespace RezervasyonApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<Resource> Resources => Set<Resource>();

    public DbSet<Driver> Drivers => Set<Driver>();

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    public DbSet<Shift> Shifts => Set<Shift>();





    protected override void OnModelCreating(ModelBuilder b)
    {   
        b.Entity<Reservation>()
         .HasIndex(x => new { x.Resource, x.StartAt }) // Rezervasyonlar için kaynak ve başlangıç zamanı benzersiz
         .IsUnique();
        
        b.Entity<Resource>()
        .HasIndex(r => r.Name)              //koltuk benzersiz
        .IsUnique();

        base.OnModelCreating(b);
        
        b.Entity<Vehicle>()                 // Plaka benzersiz olsun
         .HasIndex(v => v.Plate)
         .IsUnique();
                                            // Vehicle -> Driver: istersen Cascade kalsın
        b.Entity<Vehicle>()
            .HasOne(v => v.Driver)
            .WithMany()
            .HasForeignKey(v => v.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

                                        // Shift -> Driver: NO ACTION (cascade yok)
        b.Entity<Shift>()
            .HasOne(s => s.Driver)
            .WithMany()
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.Restrict); // veya .NoAction()

                                            // Shift -> Vehicle: NO ACTION (cascade yok)
        b.Entity<Shift>()
            .HasOne(s => s.Vehicle)
            .WithMany()
            .HasForeignKey(s => s.VehicleId)
            .OnDelete(DeleteBehavior.Restrict); // veya .NoAction()

        b.Entity<Shift>().HasIndex(s => new { s.DriverId, s.DepartureTime }).IsUnique();
        b.Entity<Shift>().HasIndex(s => new { s.VehicleId, s.DepartureTime }).IsUnique();

    }
}
