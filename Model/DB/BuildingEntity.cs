using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XakUjin2026.DB
{
    public class BuildingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string? Title { get; set; }
        public string? Address { get; set; }
        public string? Alias { get; set; }
        public int? Floor { get; set; }
        public int? ApartmentCount { get; set; }
        public int? EntranceCount { get; set; }

        public List<EntranceEntity> Entrances { get; set; } = new();
    }
}
