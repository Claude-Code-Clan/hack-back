using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
public abstract class BaseRequest
{
    public QueryParameters queryParameters { get; set; }
    public string? url { get; set; } = "https://hck-api.unicorn.icu/v1/";

    protected BaseRequest(string? token = null)
    {
        this.queryParameters = new QueryParameters(token);
    }

    public async Task<T?> SendAsync<T>() where T : class
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                string url = QueryHelpers.AddQueryString(this.url!, this.queryParameters.ToDictionary());
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content);
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

public class QueryParameters
{
    public string? token { get; set; }
    public QueryParameters(string? token = null)
    {
        this.token = token;
    }

    // Преобразование в словарь для построения query-строки (?token=...).
    // Пустые значения не добавляем.
    public IDictionary<string, string?> ToDictionary()
    {
        var dict = new Dictionary<string, string?>();
        if (!string.IsNullOrWhiteSpace(token))
            dict["token"] = token;
        return dict;
    }
}