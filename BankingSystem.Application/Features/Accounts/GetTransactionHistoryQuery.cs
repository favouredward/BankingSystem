using BankingSystem.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace BankingSystem.Application.Features.Accounts
{
    // Query to retrieve the transaction history for a specific account.
    public class GetTransactionHistoryQuery : IRequest<IEnumerable<TransactionDto>>
    {
        public Guid AccountId { get; private set; }

        // SECURITY FIX 1: Property to hold the authenticated user's ID.
        public string InitiatingUserId { get; set; }

        public GetTransactionHistoryQuery(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}
