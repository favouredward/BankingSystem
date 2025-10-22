using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.DTOs
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required string UserId { get; set; }
        public required string Email { get; set; }
    }
}
