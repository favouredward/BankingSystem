using BankingSystem.Domain.DTOs;
using MediatR;
using System;

namespace BankingSystem.Application.Features.Accounts
{
    public class GetAccountDetailsQuery : IRequest<AccountDto>
    {
        public Guid AccountId { get; private set; }

        public string InitiatingUserId { get; set; }

        public GetAccountDetailsQuery(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}
