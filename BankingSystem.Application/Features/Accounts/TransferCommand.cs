using BankingSystem.Domain.DTOs;
using MediatR;
using System;

namespace BankingSystem.Application.Features.Accounts
{
    // Command to handle a transfer between two accounts.
    public class TransferCommand : IRequest<Unit>
    {
        public string SourceAccountNumber { get; private set; }
        public string DestinationAccountNumber { get; private set; }
        public decimal Amount { get; private set; }

        // SECURITY FIX 1: Property to hold the authenticated user's ID.
        public string InitiatingUserId { get; set; }

        public TransferCommand(TransferDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SourceAccountNumber) || string.IsNullOrWhiteSpace(dto.DestinationAccountNumber))
            {
                throw new ArgumentException("Both source and destination account numbers are required.");
            }
            if (dto.Amount <= 0)
            {
                throw new ArgumentException("Transfer amount must be positive.", nameof(dto.Amount));
            }

            SourceAccountNumber = dto.SourceAccountNumber;
            DestinationAccountNumber = dto.DestinationAccountNumber;
            Amount = dto.Amount;
        }
    }
}
