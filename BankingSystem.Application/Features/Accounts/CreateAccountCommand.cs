using MediatR;
using System;
using BankingSystem.Domain.DTOs; // Keep DTO reference for context

namespace BankingSystem.Application.Features.Accounts
{
    // The Command: now only responsible for carrying the necessary data to create the account.
    public class CreateAccountCommand : IRequest<Guid>
    {
        // The AccountNumber property setter is now public, as the Handler needs to set it.
        // It starts null/empty and is populated by the handler.
        public string AccountNumber { get; set; }

        // This is the ID of the authenticated IdentityUser, retrieved from the token in the controller.
        public string OwnerId { get; private set; }

        // The constructor is simplified to only require the OwnerId from the authenticated user.
        public CreateAccountCommand(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                throw new ArgumentException("Owner ID must be provided.", nameof(ownerId));
            }

            OwnerId = ownerId;
            // The AccountNumber will be set in the handler.
        }
    }
}
