using System;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public DateTime Date { get; private set; }
        public decimal NewBalance { get; private set; }
        public DateTime   Timestamp { get; set; }

        private Transaction() { }

        public Transaction(Guid accountId, decimal amount, TransactionType type, decimal newBalance)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            Amount = amount;
            Type = type;
            Date = DateTime.UtcNow;
            NewBalance = newBalance;
        }
    }
}