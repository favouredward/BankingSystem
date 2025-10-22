using BankingSystem.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.Services
{
    public class MockExternalPaymentService : IExternalPaymentService
    {
        // We will just log the action and return success.
        public Task<bool> InitiateWithdrawalAsync(string beneficiaryAccountNumber, decimal amount)
        {
            // In a real application, this would call a third-party API.
            Console.WriteLine($"[External Service Mock] Processing withdrawal of {amount:C} to beneficiary {beneficiaryAccountNumber}...");

            // Simulate a slight delay for network latency
            Task.Delay(100).Wait();

            // Always succeed for now.
            return Task.FromResult(true);
        }
    }
}