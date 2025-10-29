using System;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.DTOs
{
    public class TransferDto
    {
        [Required]
        public required string SourceAccountNumber { get; set; }

        [Required]
        public required string DestinationAccountNumber { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}