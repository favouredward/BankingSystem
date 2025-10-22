
using BankingSystem.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.DTOs;

namespace BankingSystem.Application.Features.Accounts
{
    // Handler for the GetTransactionHistoryQuery.
    public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, IEnumerable<TransactionDto>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetTransactionHistoryQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            // SECURITY FIX 2: Use the repository method that enforces ownership.
            // This ensures we only return transactions if the initiating user owns the account.
            var account = await _accountRepository.GetTransactionHistoryByAccountIdAndOwnerIdAsync(
                request.AccountId,
                request.InitiatingUserId
            );

            if (account == null)
            {
                // Returning null will cause the controller to return 404/Access Denied, which is secure.
                return null;
            }

            // Map the domain entities to DTOs for the API response.
            return account.Select(t => new TransactionDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Timestamp = t.Timestamp
            }).ToList();
        }
    }
}
