using System.ComponentModel.DataAnnotations;

namespace XakUjin2026.DB
{
    public class WidgetEntity
    {
        [Key]
        public int Id { get; set; }

        public int? XPosition { get; set; }
        public int? YPosition { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        public int WidgetTypeId { get; set; }
        public WidgetTypeEntity? WidgetType { get; set; }

        public int DeviceId { get; set; }
        public DeviceEntity? Device { get; set; }
    }
}
