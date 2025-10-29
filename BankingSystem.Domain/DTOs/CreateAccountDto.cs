using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.DTOs
{
    
    public class CreateAccountDto
    {
        public required string AccountNumber { get; set; }
        public required string CustomerId { get; set; } 
    }
}
