using System.ComponentModel.DataAnnotations;

namespace XakUjin2026.DB
{
    public class WidgetType
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
    }
}
