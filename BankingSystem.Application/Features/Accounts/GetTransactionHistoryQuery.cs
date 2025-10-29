using BankingSystem.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace BankingSystem.Application.Features.Accounts
{
    public class GetTransactionHistoryQuery : IRequest<IEnumerable<TransactionDto>>
    {
        public Guid AccountId { get; private set; }

        public string InitiatingUserId { get; set; }

        public GetTransactionHistoryQuery(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}
