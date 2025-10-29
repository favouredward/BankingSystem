using BankingSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    public class DepositCommandHandler : IRequestHandler<DepositCommand, Unit>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IExternalPaymentService _paymentService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<DepositCommandHandler> _logger;

        public DepositCommandHandler(
            IAccountRepository accountRepository,
            IExternalPaymentService paymentService,
            ICacheService cacheService,
            ILogger<DepositCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _paymentService = paymentService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Unit> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deposit process started for account {AccountNumber} by user {UserId}", 
                    request.AccountNumber, request.InitiatingUserId);

                var account = await _accountRepository.GetByAccountNumberAndOwnerIdAsync(
                    request.AccountNumber,
                    request.InitiatingUserId);

                if (account == null)
                {
                    _logger.LogWarning("Deposit failed — Account not found or access denied for user {UserId}", 
                        request.InitiatingUserId);
                    throw new InvalidOperationException("Account not found or access denied.");
                }

                account.Deposit(request.Amount);
                _logger.LogInformation("Deposit of {Amount} applied to account {AccountNumber}. New balance: {Balance}", 
                    request.Amount, request.AccountNumber, account.Balance);

                await _accountRepository.SaveChangesAsync();
                _logger.LogInformation("Database successfully updated for deposit on account {AccountNumber}", 
                    request.AccountNumber);

                await _cacheService.RemoveAsync($"account:{account.Id}:{request.InitiatingUserId}");
                await _cacheService.RemoveAsync($"transactions:{account.Id}:{request.InitiatingUserId}");
                _logger.LogInformation("Cache invalidated for account {AccountId}", account.Id);

                _logger.LogInformation("✅ Deposit completed successfully for account {AccountNumber}", 
                    request.AccountNumber);

                return Unit.Value;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Deposit failed due to a business rule violation: {Message}", ex.Message);
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unhandled exception during deposit for account {AccountNumber}", 
                    request.AccountNumber);
                throw new ApplicationException("An unexpected error occurred while processing your deposit.");
            }
        }
    }
}
