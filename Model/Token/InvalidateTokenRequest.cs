using System.ComponentModel.DataAnnotations;

namespace XakUjin2026.Model.Token{
    public class InvalidateTokenRequest
    {
        public string? currentToken { get; set; }
    }
}
