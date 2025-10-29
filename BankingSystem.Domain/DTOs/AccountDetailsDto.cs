using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.DTOs
{
    public class AccountDetailsDto
    {
        public Guid Id { get; set; }
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public Guid OwnerId { get; set; }
    }
}
