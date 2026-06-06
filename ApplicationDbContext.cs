using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using XakUjin2026.DB;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<ApplicationUserEntity> Users { get; set; }
    public DbSet<InvalidTokenEntity> InvalidTokens { get; set; }
    public DbSet<DeviceTypeEntity> DeviceTypes { get; set; }
    public DbSet<BuildingEntity> Buildings { get; set; }
    public DbSet<EntranceEntity> Entrances { get; set; }
    public DbSet<DeviceEntity> Devices { get; set; }
    public DbSet<WidgetTypeEntity> WidgetTypes { get; set; }
    public DbSet<WidgetEntity> Widgets { get; set; }
    public DbSet<RSSLinksEntity> RSSLinks { get; set; }
    public DbSet<RSSWidgetEntity> RSSWidgets { get; set; }
    public DbSet<AlertEntity> Alerts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertEntity>()
            .Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<AlertEntity>()
            .HasOne(a => a.Device)
            .WithMany()
            .HasForeignKey(a => a.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<WidgetTypeEntity>()
            .HasIndex(w => w.Title)
            .IsUnique();

        modelBuilder.Entity<WidgetEntity>()
            .HasIndex(w => new { w.DeviceId, w.WidgetTypeId });

        // Внешний ключ: виджет -> тип виджета.
        modelBuilder.Entity<WidgetEntity>()
            .HasOne(w => w.WidgetType)
            .WithMany()
            .HasForeignKey(w => w.WidgetTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WidgetEntity>()
            .HasOne(w => w.Device)
            .WithMany(d => d.Widgets)
            .HasForeignKey(w => w.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RSSWidgetEntity>()
            .HasKey(rw => new { rw.IdRss, rw.IdWidget });

        modelBuilder.Entity<RSSWidgetEntity>()
            .HasOne(rw => rw.Rss)
            .WithMany(r => r.RSSWidgets)
            .HasForeignKey(rw => rw.IdRss)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RSSWidgetEntity>()
            .HasOne(rw => rw.Widget)
            .WithMany()
            .HasForeignKey(rw => rw.IdWidget)
            .OnDelete(DeleteBehavior.Cascade);

        // Название RSS-ссылки уникально.
        modelBuilder.Entity<RSSLinksEntity>()
            .HasIndex(r => r.Title)
            .IsUnique();
    }

}