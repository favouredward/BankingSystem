// File: BankingSystem.Domain/Entities/BaseEntity.cs

using System;

namespace BankingSystem.Domain.Entities
{
    // BaseEntity provides common properties for all domain entities.
    public abstract class BaseEntity
    {
        // Primary Key for all entities.
        public Guid Id { get; set; }
    }
}