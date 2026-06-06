namespace XakUjin2026
{
    public class ComplexListExtRequest : BaseRequest
    {
        private string? path = "complex/list";

        public ComplexListExtRequest(string? token = null) : base(token)
        {
            this.url += path;
        }

        public async Task<ComplexListResponse?> SendAsync()
        {
            return await SendAsync<ComplexListResponse>();
        }
    }
}