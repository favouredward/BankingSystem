using System;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Application.Features.Accounts;
using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.DTOs;
using BankingSystem.Domain.Entities;
using Moq;
using Microsoft.Extensions.Logging;
using Xunit;
using FluentAssertions;

namespace BankingSystem.Tests.Features.Accounts
{
    public class DepositCommandTests
    {
        private readonly Mock<IAccountRepository> _mockRepo;
        private readonly Mock<IExternalPaymentService> _mockPaymentService;
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<ILogger<DepositCommandHandler>> _mockLogger;
        private readonly DepositCommandHandler _handler;

        public DepositCommandTests()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _mockPaymentService = new Mock<IExternalPaymentService>();
            _mockCache = new Mock<ICacheService>();
            _mockLogger = new Mock<ILogger<DepositCommandHandler>>();

            _handler = new DepositCommandHandler(
                _mockRepo.Object,
                _mockPaymentService.Object,
                _mockCache.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldDepositSuccessfully_WhenAccountExists()
        {
            var ownerId = Guid.NewGuid().ToString();
            var account = new Account(ownerId, "1234567890");

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

            await _handler.Handle(command, CancellationToken.None);

            account.Balance.Should().Be(200m);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"account:{account.Id}:{ownerId}"), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"transactions:{account.Id}:{ownerId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenAccountNotFound()
        {
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

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Account not found or access denied.");
        }
    }
}
