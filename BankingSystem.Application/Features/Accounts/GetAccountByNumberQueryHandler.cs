using BankingSystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using BankingSystem.Domain.DTOs;


namespace BankingSystem.Application.Features.Accounts
{
    public class GetAccountByNumberQueryHandler : IRequestHandler<GetAccountByNumberQuery, AccountDetailsDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDistributedCache _cache;
 
        public GetAccountByNumberQueryHandler(IAccountRepository accountRepository, IDistributedCache cache) // Add IDistributedCache to the constructor
        {
            _accountRepository = accountRepository;
            _cache = cache;
        }

        public async Task<AccountDetailsDto> Handle(GetAccountByNumberQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"AccountDetails_ByNumber_{request.AccountNumber}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                
                return JsonSerializer.Deserialize<AccountDetailsDto>(cachedData)!;
            }

            var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber);

            if (account == null)
            {
                throw new InvalidOperationException("Account not found.");
            }

            var accountDto = new AccountDetailsDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                OwnerId = Guid.TryParse(account.OwnerId, out var ownerGuid) ? ownerGuid : Guid.Empty
            };

            var serializedData = JsonSerializer.Serialize(accountDto);
            await _cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return accountDto;
        }
    }
}