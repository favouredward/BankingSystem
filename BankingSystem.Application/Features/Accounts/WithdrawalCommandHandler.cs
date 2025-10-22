using BankingSystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.DTOs;

namespace BankingSystem.Application.Features.Accounts
{
    // Handler for the WithdrawalCommand.
    public class WithdrawalCommandHandler : IRequestHandler<WithdrawalCommand, Unit>
    {
        private readonly IAccountRepository _accountRepository;

        public WithdrawalCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Unit> Handle(WithdrawalCommand request, CancellationToken cancellationToken)
        {
            // SECURITY FIX 2: Use the new repository method to ensure the account exists AND 
            // belongs to the user initiating the action.
            var account = await _accountRepository.GetByAccountNumberAndOwnerIdAsync(
                request.AccountNumber,
                request.InitiatingUserId
            );

            if (account == null)
            {
                // Return a generic error message for security.
                throw new InvalidOperationException("Account not found or access denied.");
            }

            // Apply the business rule from the Domain layer (includes checking for insufficient funds).
            account.Withdraw(request.Amount);

            // Add a transaction record (optional)
            // account.AddTransaction(request.Amount, TransactionType.Withdrawal);

            await _accountRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
