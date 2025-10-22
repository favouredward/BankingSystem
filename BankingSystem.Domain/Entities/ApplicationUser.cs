// File: BankingSystem.Domain/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Domain.Entities
{
    // Custom user entity derived from IdentityUser. 
    // This will be used by the Identity system for authentication and authorization.
    public class ApplicationUser : IdentityUser
    {
        // Custom properties can be added here if needed later (e.g., FirstName, DateJoined).
    }
}