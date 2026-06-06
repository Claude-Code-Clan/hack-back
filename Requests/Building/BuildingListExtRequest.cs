namespace XakUjin2026
{
    public class BuildingListExtRequest : BaseRequest
    {
        private string? path = "buildings/get-list-crm";

        public BuildingListExtRequest(string? token = null) : base(token)
        {
            this.url += path;
        }

        public async Task<BuildingListResponse?> SendAsync()
        {
            return await SendAsync<BuildingListResponse>();
        }
    }
}
