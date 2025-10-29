using BankingSystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
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
            var account = await _accountRepository.GetByAccountNumberAndOwnerIdAsync(
                request.AccountNumber,
                request.InitiatingUserId);

            if (account == null)
                throw new InvalidOperationException("Account not found or access denied.");

            account.Withdraw(request.Amount);

            await _accountRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"account:{account.Id}:{request.InitiatingUserId}");
            await _cacheService.RemoveAsync($"transactions:{account.Id}:{request.InitiatingUserId}");

            return Unit.Value;
        }
    }
}
