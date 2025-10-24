using System;
using System.Collections.Generic;
using BankingSystem.Domain.DTOs;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Tests.TestHelpers
{
    /// <summary>
    /// Centralized helper to generate reusable fake test data
    /// for DTOs and Domain entities across all test classes.
    /// </summary>
    public static class FakeDataHelper
    {
        private static readonly Random _random = new Random();

        // ----------- COMMON GENERATORS -----------

        public static string GenerateAccountNumber()
        {
            return _random.Next(1000000000, 1999999999).ToString();
        }

        public static string GenerateOwnerId()
        {
            // Using GUID as string because your domain uses string-based owner IDs.
            return Guid.NewGuid().ToString();
        }

        // ----------- DTO BUILDERS -----------

        public static DepositDto GetDepositDto(decimal amount = 500m)
        {
            return new DepositDto
            {
                AccountNumber = GenerateAccountNumber(),
                Amount = amount
            };
        }

        public static WithdrawalDto GetWithdrawalDto(decimal amount = 100m)
        {
            return new WithdrawalDto
            {
                AccountNumber = GenerateAccountNumber(),
                Amount = amount
            };
        }

        public static TransferDto GetTransferDto(decimal amount = 300m)
        {
            return new TransferDto
            {
                SourceAccountNumber = GenerateAccountNumber(),
                DestinationAccountNumber = GenerateAccountNumber(),
                Amount = amount
            };
        }

        public static RegisterUserDto GetRegisterUserDto()
        {
            return new RegisterUserDto
            {
                Email = $"user{_random.Next(1000, 9999)}@test.com",
                Password = "Test@123"
            };
        }

        public static LoginDto GetLoginDto(string email = null, string password = "Test@123")
        {
            return new LoginDto
            {
                Email = email ?? $"login{_random.Next(1000, 9999)}@mail.com",
                Password = password
            };
        }

        // ----------- DOMAIN ENTITY BUILDERS -----------

        public static Account CreateFakeAccount(decimal initialBalance = 1000m)
        {
            var ownerId = GenerateOwnerId();
            var accountNumber = GenerateAccountNumber();
            var account = new Account(ownerId, accountNumber);

            if (initialBalance > 0)
                account.Deposit(initialBalance);

            return account;
        }

        public static Account CreateFakeAccount(string ownerId, string accountNumber, decimal balance)
        {
            var account = new Account(ownerId, accountNumber);

            if (balance > 0)
                account.Deposit(balance);

            return account;
        }

        public static Transaction CreateFakeTransaction(Guid accountId, decimal amount, TransactionType type, decimal newBalance)
        {
            return new Transaction(accountId, amount, type, newBalance);
        }

        public static IEnumerable<Transaction> CreateFakeTransactions(Guid accountId, int count = 3)
        {
            var transactions = new List<Transaction>();
            decimal runningBalance = 0;

            for (int i = 0; i < count; i++)
            {
                var amount = _random.Next(50, 500);
                runningBalance += amount;

                transactions.Add(new Transaction(
                    accountId,
                    amount,
                    TransactionType.Deposit,
                    runningBalance
                ));
            }

            return transactions;
        }

        // ----------- ACCOUNT PAIRS -----------

        public static (Account source, Account destination) CreateTransferAccounts()
        {
            var sourceOwnerId = GenerateOwnerId();
            var destOwnerId = GenerateOwnerId();

            var sourceAccount = CreateFakeAccount(sourceOwnerId, GenerateAccountNumber(), 1000m);
            var destAccount = CreateFakeAccount(destOwnerId, GenerateAccountNumber(), 0m);

            return (sourceAccount, destAccount);
        }
    }
}
