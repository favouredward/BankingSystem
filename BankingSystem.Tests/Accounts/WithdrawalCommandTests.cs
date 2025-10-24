using System;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Application.Features.Accounts;
using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.DTOs;
using BankingSystem.Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;

namespace BankingSystem.Tests.Features.Accounts
{
    public class WithdrawalCommandTests
    {
        private readonly Mock<IAccountRepository> _mockRepo;
        private readonly Mock<IExternalPaymentService> _mockPaymentService;
        private readonly Mock<ICacheService> _mockCache;
        private readonly WithdrawalCommandHandler _handler;

        public WithdrawalCommandTests()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _mockPaymentService = new Mock<IExternalPaymentService>();
            _mockCache = new Mock<ICacheService>();

            _handler = new WithdrawalCommandHandler(
                _mockRepo.Object,
                _mockPaymentService.Object,
                _mockCache.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldWithdrawSuccessfully_WhenFundsSufficient()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var account = new Account(ownerId, "9876543210");
            account.Deposit(500m);

            _mockRepo.Setup(r => r.GetByAccountNumberAndOwnerIdAsync("9876543210", ownerId))
                     .ReturnsAsync(account);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new WithdrawalDto
            {
                AccountNumber = "9876543210",
                Amount = 200m
            };
            var command = new WithdrawalCommand(dto) { InitiatingUserId = ownerId };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            account.Balance.Should().Be(300m);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"account:{account.Id}:{ownerId}"), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"transactions:{account.Id}:{ownerId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenAccountNotFound()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            _mockRepo.Setup(r => r.GetByAccountNumberAndOwnerIdAsync("nope", ownerId))
                     .ReturnsAsync((Account)null);

            var dto = new WithdrawalDto
            {
                AccountNumber = "nope",
                Amount = 100m
            };
            var command = new WithdrawalCommand(dto) { InitiatingUserId = ownerId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Account not found or access denied.");
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenInsufficientFunds()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var account = new Account(ownerId, "1111111111"); // balance = 0
            _mockRepo.Setup(r => r.GetByAccountNumberAndOwnerIdAsync("1111111111", ownerId))
                     .ReturnsAsync(account);

            var dto = new WithdrawalDto { AccountNumber = "1111111111", Amount = 50m };
            var command = new WithdrawalCommand(dto) { InitiatingUserId = ownerId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Insufficient funds.");
        }
    }
}
