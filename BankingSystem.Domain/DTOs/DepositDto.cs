using System;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.DTOs
{
    // DTO to carry deposit data from the API request.
    public class DepositDto
    {
        [Required]
        public required string AccountNumber { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}