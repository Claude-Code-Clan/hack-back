using System.ComponentModel.DataAnnotations;

namespace XakUjin2026.DB
{
    public class RSSWidgetEntity
    {
        public int IdRss { get; set; }
        public RSSLinksEntity? Rss { get; set; }

        public int IdWidget { get; set; }
        public WidgetEntity? Widget { get; set; }
    }
}
