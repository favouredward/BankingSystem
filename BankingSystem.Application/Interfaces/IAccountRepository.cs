using BankingSystem.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BankingSystem.Application.Interfaces
{
    // IGenericRepository<Account> is assumed to be implemented or implicitly used.
    public interface IAccountRepository 
    {
        Task<Account> GetByIdAsync(Guid id);
        Task<Account> GetByAccountNumberAsync(string accountNumber);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task SaveChangesAsync();

        // ----------------------------------------------------------------------
        // SECURITY FIXES: New methods to enforce ownership when retrieving an account.
        // ----------------------------------------------------------------------

        /// <summary>
        /// Retrieves an account only if its ID matches AND it belongs to the specified owner.
        /// </summary>
        Task<Account> GetByIdAndOwnerIdAsync(Guid id, string ownerId);

        /// <summary>
        /// Retrieves an account only if its account number matches AND it belongs to the specified owner.
        /// </summary>
        Task<Account> GetByAccountNumberAndOwnerIdAsync(string accountNumber, string ownerId);

        /// <summary>
        /// Retrieves all accounts belonging to a specific owner.
        /// </summary>
        Task<List<Account>> GetAccountsByOwnerIdAsync(string ownerId);
        
        /// <summary>
        /// Retrieves transactions for a specific account, filtered by owner.
        /// </summary>
        Task<ICollection<Transaction>> GetTransactionHistoryByAccountIdAndOwnerIdAsync(Guid accountId, string ownerId);
    }
}
