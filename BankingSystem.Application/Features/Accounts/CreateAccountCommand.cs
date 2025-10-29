using MediatR;
using System;
using BankingSystem.Domain.DTOs; 

namespace BankingSystem.Application.Features.Accounts
{
    
    public class CreateAccountCommand : IRequest<Guid>
    {
        public string AccountNumber { get; set; }

       
        public string OwnerId { get; private set; }
        public CreateAccountCommand(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                throw new ArgumentException("Owner ID must be provided.", nameof(ownerId));
            }

            OwnerId = ownerId;
            
        }
    }
}
