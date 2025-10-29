using BankingSystem.Domain.DTOs;
using MediatR;

namespace BankingSystem.Application.Features.Accounts
{
    public class GetAccountByNumberQuery : IRequest<AccountDetailsDto>
    {
        public string AccountNumber { get; private set; }

        public GetAccountByNumberQuery(string accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }
}