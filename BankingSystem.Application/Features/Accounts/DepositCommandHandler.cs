using BankingSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    /// <summary>
    /// Handles deposit requests into a user's account.
    /// </summary>
    public class DepositCommandHandler : IRequestHandler<DepositCommand, Unit>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IExternalPaymentService _paymentService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<DepositCommandHandler> _logger;

        public DepositCommandHandler(
            IAccountRepository accountRepository,
            IExternalPaymentService paymentService,
            ICacheService cacheService, ILogger<DepositCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _paymentService = paymentService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Unit> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            // Validate account ownership
            var account = await _accountRepository.GetByAccountNumberAndOwnerIdAsync(
                request.AccountNumber,
                request.InitiatingUserId);

            if (account == null)
                throw new InvalidOperationException("Account not found or access denied.");

            // Apply deposit domain logic
            account.Deposit(request.Amount);

            // Save changes to DB
            await _accountRepository.SaveChangesAsync();

            // ✅ Invalidate related caches (account & transactions)
            await _cacheService.RemoveAsync($"account:{account.Id}:{request.InitiatingUserId}");
            await _cacheService.RemoveAsync($"transactions:{account.Id}:{request.InitiatingUserId}");

            return Unit.Value;
        }
    }
}
