using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem.Application.Features.Accounts
{
    /// <summary>
    /// Retrieves a user's account transaction history.
    /// Implements Redis caching and ownership validation.
    /// </summary>
    public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, IEnumerable<TransactionDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetTransactionHistoryQueryHandler> _logger;

        public GetTransactionHistoryQueryHandler(
            IAccountRepository accountRepository,
            ICacheService cacheService,
            ILogger<GetTransactionHistoryQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"transactions:{request.AccountId}:{request.InitiatingUserId}";

            _logger.LogInformation("Fetching transaction history for AccountId: {AccountId}", request.AccountId);

            // ✅ 1. Check cache first
            var cachedTransactions = await _cacheService.GetAsync<IEnumerable<TransactionDto>>(cacheKey);
            if (cachedTransactions != null)
            {
                _logger.LogInformation("Cache hit for AccountId: {AccountId}", request.AccountId);
                return cachedTransactions;
            }

            // ✅ 2. Fetch securely from DB (ownership check)
            var transactions = await _accountRepository.GetTransactionHistoryByAccountIdAndOwnerIdAsync(request.AccountId, request.InitiatingUserId);
            if (transactions == null)
            {
                _logger.LogWarning("No transactions found or access denied for AccountId: {AccountId}", request.AccountId);
                return null;
            }

            // ✅ 3. Map to DTO
            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Timestamp = t.Timestamp,
                NewBalance = t.NewBalance
            }).ToList();

            // ✅ 4. Cache result for 3 minutes
            await _cacheService.SetAsync(cacheKey, transactionDtos, TimeSpan.FromMinutes(3));
            _logger.LogInformation("Transaction history cached for AccountId: {AccountId}", request.AccountId);

            return transactionDtos;
        }
    }
}
