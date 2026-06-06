using System.ComponentModel.DataAnnotations;

namespace XakUjin2026.DB
{
    public class DeviceEntity
    {
        [Key]
        public int Id { get; set; }

        public int EntranceId { get; set; }
        public EntranceEntity? Entrance { get; set; }

        public int DeviceTypeId { get; set; }
        public DeviceTypeEntity? DeviceType { get; set; }

        public List<Widget> Widgets { get; set; } = new();
    }
}
