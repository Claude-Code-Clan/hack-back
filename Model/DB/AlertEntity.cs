using System.ComponentModel.DataAnnotations;

namespace XakUjin2026.DB
{
    public enum AlertStatus
    {
        Resolved,
        Warning,
        Skip
    }

    public class AlertEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }

        public int DeviceId { get; set; }
        public DeviceEntity? Device { get; set; }

        public AlertStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
