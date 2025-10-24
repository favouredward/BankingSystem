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
    public class TransferCommandTests
    {
        private readonly Mock<IAccountRepository> _mockRepo;
        private readonly Mock<IExternalPaymentService> _mockPaymentService;
        private readonly Mock<ICacheService> _mockCache;
        private readonly TransferCommandHandler _handler;

        public TransferCommandTests()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _mockPaymentService = new Mock<IExternalPaymentService>();
            _mockCache = new Mock<ICacheService>();

            _handler = new TransferCommandHandler(
                _mockRepo.Object,
                _mockPaymentService.Object,
                _mockCache.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldTransferFunds_WhenValidAccounts()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var source = new Account(ownerId, "SRC123");
            var destOwnerId = Guid.NewGuid().ToString();
            var dest = new Account(destOwnerId, "DEST456");

            source.Deposit(1000m);

            _mockRepo.Setup(r => r.GetByAccountNumberAndOwnerIdAsync("SRC123", ownerId))
                     .ReturnsAsync(source);
            _mockRepo.Setup(r => r.GetByAccountNumberAsync("DEST456"))
                     .ReturnsAsync(dest);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new TransferDto
            {
                SourceAccountNumber = "SRC123",
                DestinationAccountNumber = "DEST456",
                Amount = 400m
            };
            var command = new TransferCommand(dto) { InitiatingUserId = ownerId };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            source.Balance.Should().Be(600m);
            dest.Balance.Should().Be(400m);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);

            // cache invalidation checks
            _mockCache.Verify(c => c.RemoveAsync($"account:{source.Id}:{ownerId}"), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"transactions:{source.Id}:{ownerId}"), Times.Once);

            _mockCache.Verify(c => c.RemoveAsync($"account:{dest.Id}:{destOwnerId}"), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"transactions:{dest.Id}:{destOwnerId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenSourceAccountNotFound()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            _mockRepo.Setup(r => r.GetByAccountNumberAndOwnerIdAsync("BADSRC", ownerId))
                     .ReturnsAsync((Account)null);

            var dto = new TransferDto
            {
                SourceAccountNumber = "BADSRC",
                DestinationAccountNumber = "DEST",
                Amount = 100m
            };
            var command = new TransferCommand(dto) { InitiatingUserId = ownerId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Source account not found or access denied.");
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenDestinationNotFound()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var source = new Account(ownerId, "SRCX");
            _mockRepo.Setup(r => r.GetByAccountNumberAndOwnerIdAsync("SRCX", ownerId))
                     .ReturnsAsync(source);
            _mockRepo.Setup(r => r.GetByAccountNumberAsync("NONEX"))
                     .ReturnsAsync((Account)null);

            var dto = new TransferDto
            {
                SourceAccountNumber = "SRCX",
                DestinationAccountNumber = "NONEX",
                Amount = 50m
            };
            var command = new TransferCommand(dto) { InitiatingUserId = ownerId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Destination account not found.");
        }
    }
}
