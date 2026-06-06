namespace XakUjin2026
{
    public class EntranceResponse
    {
        public int? id { get; set; }
        public string? entranceNumber { get; set; }
        public int buildingId { get; set; }
        public List<DeviceResponse>? devices { get; set; }
    }
}