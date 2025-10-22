using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.DTOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace BankingSystem.Application.Features.Accounts
{
    // Handler for the GetAccountDetailsQuery.
    public class GetAccountDetailsQueryHandler : IRequestHandler<GetAccountDetailsQuery, AccountDto>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountDetailsQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDto> Handle(GetAccountDetailsQuery request, CancellationToken cancellationToken)
        {
            // SECURITY FIX 2: Use the new repository method to ensure the account exists AND 
            // belongs to the user initiating the query.
            var account = await _accountRepository.GetByIdAndOwnerIdAsync(
                request.AccountId,
                request.InitiatingUserId
            );

            if (account == null)
            {
                // Returning null will cause the controller to return 404/Access Denied, which is secure.
                return null;
            }

            // Map the domain entity to the DTO for the API response.
            return new AccountDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                OwnerId = account.OwnerId // Note: The controller may filter this out later for security
            };
        }
    }
}
