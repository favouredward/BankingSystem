using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.DTOs
{
    // A simple DTO to receive account creation data from the API request.
    public class CreateAccountDto
    {
        public required string AccountNumber { get; set; }
        public required string CustomerId { get; set; } // We will parse this to a Guid later
    }
}
