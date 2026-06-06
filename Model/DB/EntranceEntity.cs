using System.ComponentModel.DataAnnotations;

namespace XakUjin2026
{
    public class EntranceEntity
    {
        [Key]
        public int Id { get; set; }

        public int BuildingId { get; set; }
        public BuildingEntity? Building { get; set; }

        public int? Number { get; set; }
        public int? FirstApartment { get; set; }
        public int? LastApartment { get; set; }

        public List<DeviceEntity> Devices { get; set; } = new();
    }
}
