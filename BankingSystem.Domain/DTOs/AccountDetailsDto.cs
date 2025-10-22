using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.DTOs
{
    // DTO to hold the account details we want to return from the API.
    public class AccountDetailsDto
    {
        public Guid Id { get; set; }
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public Guid OwnerId { get; set; }
    }
}
