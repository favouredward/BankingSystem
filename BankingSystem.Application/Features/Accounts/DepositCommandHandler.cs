using BankingSystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.Features.Accounts
{
    // Handler for the DepositCommand.
    public class DepositCommandHandler : IRequestHandler<DepositCommand, Unit>
    {
        private readonly IAccountRepository _accountRepository;

        public DepositCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Unit> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            // SECURITY FIX 2: Use the new repository method to ensure the account exists AND 
            // belongs to the user initiating the action (OwnerId == InitiatingUserId).
            var account = await _accountRepository.GetByAccountNumberAndOwnerIdAsync(
                request.AccountNumber,
                request.InitiatingUserId
            );

            if (account == null)
            {
                // Return a generic error message for security.
                throw new InvalidOperationException("Account not found or access denied.");
            }

            // Apply the business rule from the Domain layer.
            account.Deposit(request.Amount);

            // Add a transaction record (optional, but good practice if transactions entity is used)
            // account.AddTransaction(request.Amount, TransactionType.Deposit);

            await _accountRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
