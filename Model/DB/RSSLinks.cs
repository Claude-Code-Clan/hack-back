using System.ComponentModel.DataAnnotations;

namespace XakUjin2026.DB
{
    public class RSSLinksEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Link { get; set; }
        public List<RSSWidgetEntity> RSSWidgets { get; set; } = new();
    }
}
