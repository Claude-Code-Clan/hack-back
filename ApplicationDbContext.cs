using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using XakUjin2026;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<InvalidToken> InvalidTokens { get; set; }
    public DbSet<DeviceType> DeviceTypes { get; set; }
    public DbSet<BuildingEntity> Buildings { get; set; }
    public DbSet<EntranceEntity> Entrances { get; set; }
    public DbSet<DeviceEntity> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Подъезд уникален в рамках здания по номеру — для upsert при синхронизации.
        modelBuilder.Entity<EntranceEntity>()
            .HasIndex(e => new { e.BuildingId, e.Number })
            .IsUnique();

        // Устройство уникально в рамках подъезда по типу — у подъезда не более одного устройства каждого типа.
        modelBuilder.Entity<DeviceEntity>()
            .HasIndex(d => new { d.EntranceId, d.DeviceTypeId })
            .IsUnique();

        // modelBuilder.Entity<Apartment>()
        //     .HasOne(a => a.Home)
        //     .WithMany(h => h.Apartments)
        //     .HasForeignKey(a => a.HomeId);

        // modelBuilder.Entity<Apartment>()
        //     .HasOne(a => a.ApplicationUser)
        //     .WithMany()
        //     .HasForeignKey(a => a.UserId)
        //     .IsRequired(false);

        // modelBuilder.Entity<Signal>()
        //     .HasOne(a => a.Apartment)
        //     .WithMany(h => h.Signals)
        //     .HasForeignKey(a => a.ApartmentId);

        // modelBuilder.Entity<Indication>()
        //     .HasOne(a => a.Signal)
        //     .WithMany(h => h.Indications)
        //     .HasForeignKey(a => a.SignalId);

        // modelBuilder.Entity<Signal>()
        //    .Property(s => s.SignalId)
        //    .ValueGeneratedOnAdd();
    }

}