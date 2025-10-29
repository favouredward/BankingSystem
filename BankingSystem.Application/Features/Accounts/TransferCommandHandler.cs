using BankingSystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Unit>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IExternalPaymentService _paymentService;
        private readonly ICacheService _cacheService;

        public TransferCommandHandler(
            IAccountRepository accountRepository,
            IExternalPaymentService paymentService,
            ICacheService cacheService)
        {
            _accountRepository = accountRepository;
            _paymentService = paymentService;
            _cacheService = cacheService;
        }

        public async Task<Unit> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            var sourceAccount = await _accountRepository.GetByAccountNumberAndOwnerIdAsync(
                request.SourceAccountNumber,
                request.InitiatingUserId);

            if (sourceAccount == null)
                throw new InvalidOperationException("Source account not found or access denied.");

            var destinationAccount = await _accountRepository.GetByAccountNumberAsync(request.DestinationAccountNumber);

            if (destinationAccount == null)
                throw new InvalidOperationException("Destination account not found.");

            if (sourceAccount.AccountNumber == destinationAccount.AccountNumber)
                throw new InvalidOperationException("Cannot transfer funds to the same account.");

            sourceAccount.Transfer(destinationAccount, request.Amount);

            await _accountRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"account:{sourceAccount.Id}:{request.InitiatingUserId}");
            await _cacheService.RemoveAsync($"transactions:{sourceAccount.Id}:{request.InitiatingUserId}");

            await _cacheService.RemoveAsync($"account:{destinationAccount.Id}:{destinationAccount.OwnerId}");
            await _cacheService.RemoveAsync($"transactions:{destinationAccount.Id}:{destinationAccount.OwnerId}");

            return Unit.Value;
        }
    }
}
 