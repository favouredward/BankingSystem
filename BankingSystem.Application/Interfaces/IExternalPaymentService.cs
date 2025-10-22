using System;
using System.Threading.Tasks;

namespace BankingSystem.Application.Interfaces
{
    public interface IExternalPaymentService
    {
        // Simulates sending money to an external party (like a third-party withdrawal).
        Task<bool> InitiateWithdrawalAsync(string beneficiaryAccountNumber, decimal amount);

        // Simulates receiving money from an external party (for deposits, we might not use this).
        // Task<bool> InitiateDepositAsync(string sourceAccount, decimal amount);
    }
}