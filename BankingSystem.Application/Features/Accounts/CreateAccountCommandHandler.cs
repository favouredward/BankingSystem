using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    /// <summary>
    /// Handles creation of a new bank account for a registered user.
    /// Includes validation to ensure a user cannot have multiple accounts.
    /// </summary>
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Generates a unique, 10-digit account number.
        /// Ensures it doesn’t already exist in the system.
        /// </summary>
        private async Task<string> GenerateUniqueAccountNumber()
        {
            const int maxAttempts = 10;
            var random = new Random();

            for (int i = 0; i < maxAttempts; i++)
            {
                string newAccountNumber = random.Next(1000000000, 2100000000).ToString();

                var existingAccount = await _accountRepository.GetByAccountNumberAsync(newAccountNumber);
                if (existingAccount == null)
                {
                    return newAccountNumber;
                }
            }

            throw new InvalidOperationException("Failed to generate a unique account number after multiple attempts.");
        }

        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // ✅ STEP 1: Check if this user already has an account
            var existingAccounts = await _accountRepository.GetAccountsByOwnerIdAsync(request.OwnerId);
            if (existingAccounts != null && existingAccounts.Count > 0)
            {
                // User already owns an account — return its existing ID
                var existingAccount = existingAccounts[0];
                return existingAccount.Id;
            }

            // ✅ STEP 2: Generate a unique account number
            string newAccountNumber = await GenerateUniqueAccountNumber();
            request.AccountNumber = newAccountNumber;

            // ✅ STEP 3: Create the new account entity
            var account = new Account(request.OwnerId, request.AccountNumber);

            // ✅ STEP 4: Persist to database
            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();

            // ✅ STEP 5: Return the created account ID
            return account.Id;
        }
    }
}
