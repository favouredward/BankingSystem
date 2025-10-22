// File: BankingSystem.Domain/Enums/TransactionType.cs

namespace BankingSystem.Domain.Enums
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        TransferIn,
        TransferOut,
        // Used for account creation, often recorded as 0 or simply not logged
        InitialBalance 
    }
}