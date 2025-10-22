// File: BankingSystem.Infrastructure/Repositories/AccountRepository.cs

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
    // This class implements the IAccountRepository interface, now with strong ownership checks.
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingSystemDbContext _dbContext;

        public AccountRepository(BankingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves an account by its GUID ID, enforcing ownership via the provided ownerId.
        /// Used for read operations like GetAccountDetails.
        /// </summary>
        public async Task<Account> GetByIdAndOwnerIdAsync(Guid id, string ownerId)
        {
            // Security check: The account ID MUST match the authenticated OwnerId.
            return await _dbContext.Accounts
                // Load recent transactions for the DTO mapping
                .Include(a => a.Transactions.OrderByDescending(t => t.Timestamp).Take(10))
                .FirstOrDefaultAsync(a => a.Id == id && a.OwnerId == ownerId);
        }

        /// <summary>
        /// Retrieves an account by its AccountNumber, enforcing ownership via the provided ownerId.
        /// Used for transaction operations (Deposit/Withdrawal) where the user owns the source account.
        /// </summary>
        public async Task<Account> GetByAccountNumberAndOwnerIdAsync(string accountNumber, string ownerId)
        {
            // Security check: The account number MUST match the authenticated OwnerId.
            return await _dbContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.OwnerId == ownerId);
        }

        /// <summary>
        /// Retrieves an account by its AccountNumber without enforcing ownership.
        /// This is used safely ONLY for the Destination Account during a Transfer.
        /// </summary>
        public async Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
            return await _dbContext.Accounts
               .Include(a => a.Transactions)
               .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        // --- BASIC CRUD OPERATIONS ---

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            // Note: In a real app, this should be scoped by authenticated user's ID.
            return await _dbContext.Accounts.ToListAsync();
        }

        public async Task AddAsync(Account account)
        {
            await _dbContext.Accounts.AddAsync(account);
        }

        public Task UpdateAsync(Account account)
        {
            // EF Core tracks changes, so no explicit operation is usually needed here, 
            // but calling Update ensures the context is aware if detached.
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

        public Task<Account> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task IAccountRepository.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }

        public Task<List<Account>> GetAccountsByOwnerIdAsync(string ownerId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetTransactionHistoryByAccountIdAndOwnerIdAsync(Guid accountId, string ownerId)
        {
            throw new NotImplementedException();
        }
    }
}