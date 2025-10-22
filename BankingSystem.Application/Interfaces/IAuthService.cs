using BankingSystem.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Error)> RegisterUserAsync(RegisterUserDto dto);
        Task<(bool Success, string Token, string UserId, string Error)> LoginAsync(LoginDto dto);
    }
}
