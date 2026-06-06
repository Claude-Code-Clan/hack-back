using System.ComponentModel.DataAnnotations;

namespace XakUjin2026
{
    public class AuthRequest
    {
        public string? email { get; set; }

        public string? password { get; set; }
    }
}
