using BankingSystem.Domain.DTOs;
using MediatR;
using System;

namespace BankingSystem.Application.Features.Accounts
{
    public class WithdrawalCommand : IRequest<Unit>
    {
        public string AccountNumber { get; private set; }
        public decimal Amount { get; private set; }

        public string InitiatingUserId { get; set; }

        public WithdrawalCommand(WithdrawalDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AccountNumber))
            {
                throw new ArgumentException("Account number is required.", nameof(dto.AccountNumber));
            }
            if (dto.Amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.", nameof(dto.Amount));
            }

            AccountNumber = dto.AccountNumber;
            Amount = dto.Amount;
        }
    }
}
