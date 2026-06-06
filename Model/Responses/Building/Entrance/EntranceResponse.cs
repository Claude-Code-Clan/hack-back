using XakUjin2026.Model.Responses.Building.Entrance.Device;

namespace XakUjin2026.Model.Responses.Building.Entrance{
    public class EntranceResponse
    {
        public int? id { get; set; }
        public string? entranceNumber { get; set; }
        public int buildingId { get; set; }
        public List<DeviceResponse>? devices { get; set; }
    }
}