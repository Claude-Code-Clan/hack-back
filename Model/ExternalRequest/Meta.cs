namespace XakUjin2026.Model.ExternalRequest{
    // Блок meta из data: параметры постраничной навигации.
    public class Meta
    {
        public int? current_page { get; set; }
        public int? from { get; set; }
        public int? last_page { get; set; }
        public string? path { get; set; }
        // per_page приходит строкой ("500"), поэтому тип string.
        public string? per_page { get; set; }
        public int? to { get; set; }
        public int? total { get; set; }
    }
}
