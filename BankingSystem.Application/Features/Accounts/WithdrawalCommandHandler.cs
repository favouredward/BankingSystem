using BankingSystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    /// <summary>
    /// Handles withdrawal requests from a user's account.
    /// </summary>
    public class WithdrawalCommandHandler : IRequestHandler<WithdrawalCommand, Unit>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IExternalPaymentService _paymentService;
        private readonly ICacheService _cacheService;

        public WithdrawalCommandHandler(
            IAccountRepository accountRepository,
            IExternalPaymentService paymentService,
            ICacheService cacheService)
        {
            _accountRepository = accountRepository;
            _paymentService = paymentService;
            _cacheService = cacheService;
        }

        public async Task<Unit> Handle(WithdrawalCommand request, CancellationToken cancellationToken)
        {
            // Validate ownership
            var account = await _accountRepository.GetByAccountNumberAndOwnerIdAsync(
                request.AccountNumber,
                request.InitiatingUserId);

            if (account == null)
                throw new InvalidOperationException("Account not found or access denied.");

            // Apply withdrawal domain logic (checks for insufficient funds)
            account.Withdraw(request.Amount);

            // Save to DB
            await _accountRepository.SaveChangesAsync();

            // ✅ Invalidate related caches
            await _cacheService.RemoveAsync($"account:{account.Id}:{request.InitiatingUserId}");
            await _cacheService.RemoveAsync($"transactions:{account.Id}:{request.InitiatingUserId}");

            return Unit.Value;
        }
    }
}
