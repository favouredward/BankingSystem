using System;

namespace BankingSystem.Domain.DTOs
{
    
    public class AccountDto
    {
        public Guid Id { get; set; }
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public required string OwnerId { get; set; }  }
}