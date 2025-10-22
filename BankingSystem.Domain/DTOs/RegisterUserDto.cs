using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }
    }
}