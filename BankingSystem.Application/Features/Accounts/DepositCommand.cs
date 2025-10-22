using BankingSystem.Domain.DTOs;
using MediatR;
using System;

namespace BankingSystem.Application.Features.Accounts
{
    // Command to handle a deposit into an account.
    public class DepositCommand : IRequest<Unit>
    {
        public string AccountNumber { get; private set; }
        public decimal Amount { get; private set; }

        // SECURITY FIX 1: Property to hold the authenticated user's ID (set in the controller).
        public string InitiatingUserId { get; set; }

        public DepositCommand(DepositDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AccountNumber))
            {
                throw new ArgumentException("Account number is required.", nameof(dto.AccountNumber));
            }
            if (dto.Amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.", nameof(dto.Amount));
            }

            AccountNumber = dto.AccountNumber;
            Amount = dto.Amount;
        }
    }
}
