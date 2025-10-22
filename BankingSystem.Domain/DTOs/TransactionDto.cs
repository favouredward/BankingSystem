using System;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public required string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal NewBalance { get; set; }
    }
}