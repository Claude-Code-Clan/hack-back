using XakUjin2026.Model.Responses.Building.Entrance;

namespace XakUjin2026.Model.Responses.Building{
    public class BuildingsResponse
    {
        public int? id { get; set; }
        public string? buildingTitle { get; set; }
        public string? buildingAddress { get; set; }
        public List<EntranceResponse>? entrances { get; set; }

        public BuildingsResponse(int? id, string? buildingTitle, string? buildingAddress, List<EntranceResponse>? entrances)
        {
            this.id = id;
            this.buildingTitle = buildingTitle;
            this.buildingAddress = buildingAddress;
            this.entrances = entrances;
        }
    }
}
