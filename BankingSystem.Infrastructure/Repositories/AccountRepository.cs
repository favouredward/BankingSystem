using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.Entities;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace BankingSystem.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingSystemDbContext _dbContext;

        public AccountRepository(BankingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

      
        public async Task<Account> GetByIdAndOwnerIdAsync(Guid id, string ownerId)
        {
            return await _dbContext.Accounts
                .Include(a => a.Transactions.OrderByDescending(t => t.Timestamp).Take(10))
                .FirstOrDefaultAsync(a => a.Id == id && a.OwnerId == ownerId);
        }

        
        public async Task<Account> GetByAccountNumberAndOwnerIdAsync(string accountNumber, string ownerId)
        {
            return await _dbContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.OwnerId == ownerId);
        }

        
        public async Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
            return await _dbContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        
        public async Task<Account> GetByIdAsync(Guid id)
        {
            return await _dbContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        
        public async Task<List<Account>> GetAccountsByOwnerIdAsync(string ownerId)
        {
            return await _dbContext.Accounts
                .Where(a => a.OwnerId == ownerId)
                .Include(a => a.Transactions)
                .ToListAsync();
        }

       
        public async Task<ICollection<Transaction>> GetTransactionHistoryByAccountIdAndOwnerIdAsync(Guid accountId, string ownerId)
        {
            var account = await _dbContext.Accounts
                .Include(a => a.Transactions.OrderByDescending(t => t.Timestamp))
                .FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerId);

            return account?.Transactions?.ToList() ?? new List<Transaction>();
        }

        
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _dbContext.Accounts
                .Include(a => a.Transactions)
                .ToListAsync();
        }

        public async Task AddAsync(Account account)
        {
            
            var existing = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.OwnerId == account.OwnerId);
            if (existing != null)
                return;

            
            account = new Account(account.OwnerId, account.AccountNumber);
            await _dbContext.Accounts.AddAsync(account);
        }

        public Task UpdateAsync(Account account)
        {
            _dbContext.Accounts.Update(account);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _dbContext.Transactions.AddAsync(transaction);
        }

      
        Task IAccountRepository.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }
    }
}
