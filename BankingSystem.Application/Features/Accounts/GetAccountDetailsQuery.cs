using BankingSystem.Domain.DTOs;
using MediatR;
using System;

namespace BankingSystem.Application.Features.Accounts
{
    // Query to retrieve account details.
    public class GetAccountDetailsQuery : IRequest<AccountDto>
    {
        public Guid AccountId { get; private set; }

        // SECURITY FIX 1: Property to hold the authenticated user's ID.
        public string InitiatingUserId { get; set; }

        public GetAccountDetailsQuery(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}
