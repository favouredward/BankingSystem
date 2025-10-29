using System;
using System.Threading.Tasks;

namespace BankingSystem.Application.Interfaces
{
    public interface IExternalPaymentService
    {
        Task<bool> InitiateWithdrawalAsync(string beneficiaryAccountNumber, decimal amount);

    }
}