// File: BankingSystem.Application/DTOs/AccountDto.cs

using System;

namespace BankingSystem.Domain.DTOs
{
    // DTO for returning account details to the client
    public class AccountDto
    {
        public Guid Id { get; set; }
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public required string OwnerId { get; set; } // Required for internal reference, though often masked externally
    }
}