using System.ComponentModel.DataAnnotations;

namespace XakUjin2026
{
    public class DeviceEntity
    {
        [Key]
        public int Id { get; set; }

        public int EntranceId { get; set; }
        public EntranceEntity? Entrance { get; set; }

        public int DeviceTypeId { get; set; }
        public DeviceType? DeviceType { get; set; }
    }
}
