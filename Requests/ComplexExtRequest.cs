using System.Text.Json;

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
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return JsonSerializer.Deserialize<ComplexListResponse>(content);
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while sending the request: {ex.Message}");
                    return null;
                }
            }
        }
    }
}