using System;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.DTOs
{
    public class WithdrawalDto
    {
        [Required]
        public required string AccountNumber { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}