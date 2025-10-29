using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

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
            var existingAccounts = await _accountRepository.GetAccountsByOwnerIdAsync(request.OwnerId);
            if (existingAccounts != null && existingAccounts.Count > 0)
            {
                var existingAccount = existingAccounts[0];
                return existingAccount.Id;
            }

            string newAccountNumber = await GenerateUniqueAccountNumber();
            request.AccountNumber = newAccountNumber;

            var account = new Account(request.OwnerId, request.AccountNumber);

            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();

            return account.Id;
        }
    }
}
