namespace XakUjin2026
{
    public class BuildingsResponse
    {
        private int? id { get; set; }
        private int? buildingNumber { get; set; }
        private string? buildingAddress { get; set; }
        private List<EntranceResponse>? entrances { get; set; }
    }
}