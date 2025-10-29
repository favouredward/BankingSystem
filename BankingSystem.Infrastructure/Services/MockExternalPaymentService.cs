using BankingSystem.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.Services
{
    public class MockExternalPaymentService : IExternalPaymentService
    {
        
        public Task<bool> InitiateWithdrawalAsync(string beneficiaryAccountNumber, decimal amount)
        {
           
            Console.WriteLine($"[External Service Mock] Processing withdrawal of {amount:C} to beneficiary {beneficiaryAccountNumber}...");

            
            Task.Delay(100).Wait();

            return Task.FromResult(true);
        }
    }
}