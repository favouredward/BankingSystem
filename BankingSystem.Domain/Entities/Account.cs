using System;
using System.Collections.Generic;
using System.Linq;
using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystem.Domain.Entities
{
    
    public class Account : BaseEntity 
    {
        public string AccountNumber { get; private set; } = string.Empty;

       
        [Column(TypeName = "decimal(18, 2)")] 
        public decimal Balance { get; private set; }
        public string OwnerId { get; private set; }
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
        private Account() { }
        public Account(string ownerId, string accountNumber)
        {
            Id = Guid.NewGuid();
            OwnerId = ownerId; 
            AccountNumber = accountNumber;
            Balance = 0; 
        }

        
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
            }
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.", nameof(amount));
            }

            if (Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }

            Balance -= amount;
        }

        public void Transfer(Account destinationAccount, decimal amount)
        {
           
            this.Withdraw(amount);

            destinationAccount.Deposit(amount);
        }



    }
}
