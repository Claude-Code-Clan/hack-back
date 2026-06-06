using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using XakUjin2026.DB;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<InvalidToken> InvalidTokens { get; set; }
    public DbSet<DeviceTypeEntity> DeviceTypes { get; set; }
    public DbSet<BuildingEntity> Buildings { get; set; }
    public DbSet<EntranceEntity> Entrances { get; set; }
    public DbSet<DeviceEntity> Devices { get; set; }
    public DbSet<WidgetType> WidgetTypes { get; set; }
    public DbSet<Widget> Widgets { get; set; }

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

        // Первичный ключ устройства.
        modelBuilder.Entity<DeviceEntity>()
            .HasKey(d => d.Id);

        // Внешний ключ: устройство -> подъезд.
        modelBuilder.Entity<DeviceEntity>()
            .HasOne(d => d.Entrance)
            .WithMany(e => e.Devices)
            .HasForeignKey(d => d.EntranceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Внешний ключ: устройство -> тип устройства.
        modelBuilder.Entity<DeviceEntity>()
            .HasOne(d => d.DeviceType)
            .WithMany()
            .HasForeignKey(d => d.DeviceTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WidgetType>()
            .HasIndex(w => w.Title)
            .IsUnique();

        modelBuilder.Entity<Widget>()
            .HasIndex(w => new { w.DeviceId, w.WidgetTypeId })
            .IsUnique();

        // Внешний ключ: виджет -> тип виджета.
        modelBuilder.Entity<Widget>()
            .HasOne(w => w.WidgetType)
            .WithMany()
            .HasForeignKey(w => w.WidgetTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Widget>()
            .HasOne(w => w.Device)
            .WithMany(d => d.Widgets)
            .HasForeignKey(w => w.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}