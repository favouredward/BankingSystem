using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    // The Handler: it now includes the logic to generate a unique account number.
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;

        // The handler gets the repository via Dependency Injection.
        public CreateAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // Helper method to generate a unique, 10-digit account number.
        private async Task<string> GenerateUniqueAccountNumber()
        {
            // Set a maximum number of attempts to prevent infinite loops 
            // if the system somehow runs out of unique numbers.
            const int maxAttempts = 10;
            var random = new Random();

            for (int i = 0; i < maxAttempts; i++)
            {
                // Generate a random number between 1,000,000,000 and 2,100,000,000 
                // to ensure a 10-digit number (starting with a 1 or 2).
                string newAccountNumber = random.Next(1000000000, 2100000000).ToString();

                // Check repository to ensure uniqueness
                var existingAccount = await _accountRepository.GetByAccountNumberAsync(newAccountNumber);

                if (existingAccount == null)
                {
                    return newAccountNumber;
                }
            }
            // If we fail after maxAttempts, throw a critical exception
            throw new InvalidOperationException("Failed to generate a unique account number after multiple attempts.");
        }


        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // STEP 1: Generate and validate a unique account number.
            string newAccountNumber = await GenerateUniqueAccountNumber();

            // Set the generated number on the command. This allows the API controller 
            // to access and return the new account number in the response payload.
            request.AccountNumber = newAccountNumber;

            // 1. We use the business rules from our Domain layer.
            // Create the new account entity using the OwnerId from the Command (JWT claim) 
            // and the system-generated account number.
            var account = new Account(request.OwnerId, request.AccountNumber);

            // 2. We use our Infrastructure layer (via the interface) to save the new account.
            await _accountRepository.AddAsync(account);

            // 3. We explicitly save the changes to the database.
            await _accountRepository.SaveChangesAsync();

            // 4. We return the ID of the newly created account.
            return account.Id;
        }
    }
}
