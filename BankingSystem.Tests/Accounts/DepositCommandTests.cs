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
    public class DepositCommandTests
    {
        private readonly Mock<IAccountRepository> _mockRepo;
        private readonly Mock<IExternalPaymentService> _mockPaymentService;
        private readonly Mock<ICacheService> _mockCache;
        private readonly DepositCommandHandler _handler;

        public DepositCommandTests()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _mockPaymentService = new Mock<IExternalPaymentService>();
            _mockCache = new Mock<ICacheService>();

            _handler = new DepositCommandHandler(
                _mockRepo.Object,
                _mockPaymentService.Object,
                _mockCache.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldDepositSuccessfully_WhenAccountExists()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var account = new Account(ownerId, "1234567890");
            // initial balance 0
            _mockRepo
                .Setup(r => r.GetByAccountNumberAndOwnerIdAsync("1234567890", ownerId))
                .ReturnsAsync(account);

            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new DepositDto
            {
                AccountNumber = "1234567890",
                Amount = 200m
            };

            var command = new DepositCommand(dto) { InitiatingUserId = ownerId };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            account.Balance.Should().Be(200m);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"account:{account.Id}:{ownerId}"), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"transactions:{account.Id}:{ownerId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenAccountNotFound()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            _mockRepo
                .Setup(r => r.GetByAccountNumberAndOwnerIdAsync("0000000000", ownerId))
                .ReturnsAsync((Account)null);

            var dto = new DepositDto
            {
                AccountNumber = "0000000000",
                Amount = 100m
            };
            var command = new DepositCommand(dto) { InitiatingUserId = ownerId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Account not found or access denied.");
        }
    }
}
