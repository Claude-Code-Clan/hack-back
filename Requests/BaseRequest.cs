public abstract class BaseRequest
{
    public string? token { get; set; }
    public string? url { get; set; } = "https://hck-api.unicorn.icu/v1/";

    protected BaseRequest(string? token = null)
    {
        this.token = token;
    }
}