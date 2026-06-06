using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace XakUjin2026
{
    public class RegistrationRequest
    {
        [Required]
        public string? username { get; set; }

        [Required]
        [EmailAddress]
        public string? email { get; set; }

        [Required]
        public string? password { get; set; }
    }

}
