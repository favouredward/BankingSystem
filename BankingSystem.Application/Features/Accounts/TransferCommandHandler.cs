using BankingSystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.Features.Accounts
{
    // Handler for the TransferCommand.
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Unit>
    {
        private readonly IAccountRepository _accountRepository;

        public TransferCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Unit> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate Source Account Ownership: We must verify the user owns the account they are withdrawing from.
            var sourceAccount = await _accountRepository.GetByAccountNumberAndOwnerIdAsync(
                request.SourceAccountNumber,
                request.InitiatingUserId
            );

            if (sourceAccount == null)
            {
                // Critical security check. Deny access if account is not found OR not owned by the user.
                throw new InvalidOperationException("Source account not found or access denied.");
            }

            // 2. Locate Destination Account: Destination account can belong to anyone.
            var destinationAccount = await _accountRepository.GetByAccountNumberAsync(request.DestinationAccountNumber);

            if (destinationAccount == null)
            {
                throw new InvalidOperationException("Destination account not found.");
            }

            if (sourceAccount.AccountNumber == destinationAccount.AccountNumber)
            {
                throw new InvalidOperationException("Cannot transfer funds to the same account.");
            }

            // 3. Perform transfer logic (uses domain logic for Withdraw/Deposit).
            sourceAccount.Transfer(destinationAccount, request.Amount);

            // Add transaction records (optional)
            // sourceAccount.AddTransaction(-request.Amount, TransactionType.TransferOut);
            // destinationAccount.AddTransaction(request.Amount, TransactionType.TransferIn);

            await _accountRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
