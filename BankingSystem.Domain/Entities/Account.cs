using System;
using System.Collections.Generic;
using System.Linq;
using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystem.Domain.Entities
{
    // A simple class representing a bank account.
    // The "Domain" layer contains the core business rules and entities.
    public class Account : BaseEntity // Assuming BaseEntity provides the Id property
    {
        // Unique account number
        public string AccountNumber { get; private set; } = string.Empty;

        // The current balance of the account
        [Column(TypeName = "decimal(18, 2)")] 
        public decimal Balance { get; private set; }

        // FOREIGN KEY FIX: The OwnerId property is a string to link directly to the IdentityUser.Id.
        public string OwnerId { get; private set; }

        // Collection property for EF Core mapping
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

        // Required for EF Core to instantiate the entity
        private Account() { }

        // Constructor to create a new account.
        public Account(string ownerId, string accountNumber)
        {
            OwnerId = ownerId; // Takes string OwnerId
            AccountNumber = accountNumber;
            Balance = 0; // New accounts start with a zero balance
        }

        // Method to handle a deposit.
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
            }
            Balance += amount;
        }

        // Method to handle a withdrawal.
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

        // Method to transfer funds from this account to another.
        public void Transfer(Account destinationAccount, decimal amount)
        {
            // First, check if we can withdraw from this account.
            this.Withdraw(amount);

            // If the withdrawal is successful, deposit into the destination account.
            destinationAccount.Deposit(amount);
        }



    }
}
