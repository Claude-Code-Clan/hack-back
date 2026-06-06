using XakUjin2026.Model.ExternalRequest.Complex;

namespace XakUjin2026.Requests.Complex
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