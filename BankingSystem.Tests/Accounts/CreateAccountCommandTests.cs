using System;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Application.Features.Accounts;
using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace BankingSystem.Tests.Features.Accounts
{
    public class CreateAccountCommandTests
    {
        private readonly Mock<IAccountRepository> _mockRepo;
        private readonly CreateAccountCommandHandler _handler;

        public CreateAccountCommandTests()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _handler = new CreateAccountCommandHandler(_mockRepo.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateAccountSuccessfully()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var command = new CreateAccountCommand(ownerId);

            _mockRepo.Setup(r => r.GetByAccountNumberAsync(It.IsAny<string>()))
                     .ReturnsAsync((Account)null); // Simulate unique account number

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            command.AccountNumber.Should().NotBeNullOrEmpty();
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUnableToGenerateUniqueAccountNumber()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var command = new CreateAccountCommand(ownerId);
            var existingAccount = new Account(ownerId, "1234567890");

            _mockRepo.Setup(r => r.GetByAccountNumberAsync(It.IsAny<string>()))
                     .ReturnsAsync(existingAccount); // Always returns an existing account

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
