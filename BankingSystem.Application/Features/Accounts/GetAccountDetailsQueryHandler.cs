using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    public class GetAccountDetailsQueryHandler : IRequestHandler<GetAccountDetailsQuery, AccountDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetAccountDetailsQueryHandler> _logger;

        public GetAccountDetailsQueryHandler(
            IAccountRepository accountRepository,
            ICacheService cacheService,
            ILogger<GetAccountDetailsQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<AccountDto> Handle(GetAccountDetailsQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"account:{request.AccountId}:{request.InitiatingUserId}";

            _logger.LogInformation("Retrieving account details for AccountId: {AccountId}", request.AccountId);

            var cachedAccount = await _cacheService.GetAsync<AccountDto>(cacheKey);
            if (cachedAccount != null)
            {
                _logger.LogInformation("Cache hit for AccountId: {AccountId}", request.AccountId);
                return cachedAccount;
            }

            var account = await _accountRepository.GetByIdAndOwnerIdAsync(request.AccountId, request.InitiatingUserId);
            if (account == null)
            {
                _logger.LogWarning("Account not found or access denied for AccountId: {AccountId}", request.AccountId);
                return null;
            }

            var accountDto = new AccountDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                OwnerId = account.OwnerId
            };

            await _cacheService.SetAsync(cacheKey, accountDto, TimeSpan.FromMinutes(5));
            _logger.LogInformation("Account details cached for AccountId: {AccountId}", request.AccountId);

            return accountDto;
        }
    }
}
