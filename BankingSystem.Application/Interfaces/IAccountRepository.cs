using BankingSystem.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BankingSystem.Application.Interfaces
{
   
    public interface IAccountRepository 
    {
        Task<Account> GetByIdAsync(Guid id);
        Task<Account> GetByAccountNumberAsync(string accountNumber);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task SaveChangesAsync();

        
        Task<Account> GetByIdAndOwnerIdAsync(Guid id, string ownerId);

      
        Task<Account> GetByAccountNumberAndOwnerIdAsync(string accountNumber, string ownerId);

        Task<List<Account>> GetAccountsByOwnerIdAsync(string ownerId);
        
      
        Task<ICollection<Transaction>> GetTransactionHistoryByAccountIdAndOwnerIdAsync(Guid accountId, string ownerId);
    }
}
