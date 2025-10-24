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
    public class GetAccountDetailsQueryTests
    {
        private readonly Mock<IAccountRepository> _mockRepo;
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<GetAccountDetailsQueryHandler>> _mockLogger;
        private readonly GetAccountDetailsQueryHandler _handler;

        public GetAccountDetailsQueryTests()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _mockCache = new Mock<ICacheService>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<GetAccountDetailsQueryHandler>>();

            _handler = new GetAccountDetailsQueryHandler(
                _mockRepo.Object,
                _mockCache.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFromCache_WhenCached()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var ownerId = Guid.NewGuid().ToString();
            var dto = new AccountDto
            {
                Id = accountId,
                AccountNumber = "ACC100",
                Balance = 100m,
                OwnerId = ownerId
            };

            _mockCache.Setup(c => c.GetAsync<AccountDto>($"account:{accountId}:{ownerId}"))
                      .ReturnsAsync(dto);

            var query = new GetAccountDetailsQuery(accountId) { InitiatingUserId = ownerId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccountNumber.Should().Be("ACC100");
            _mockRepo.Verify(r => r.GetByIdAndOwnerIdAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFromDb_AndCache_WhenNotCached()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var ownerId = Guid.NewGuid().ToString();
            var account = new Account(ownerId, "ACC200");

            _mockCache.Setup(c => c.GetAsync<AccountDto>($"account:{accountId}:{ownerId}"))
                      .ReturnsAsync((AccountDto)null);

            _mockRepo.Setup(r => r.GetByIdAndOwnerIdAsync(accountId, ownerId))
                     .ReturnsAsync(account);

            var query = new GetAccountDetailsQuery(accountId) { InitiatingUserId = ownerId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccountNumber.Should().Be("ACC200");
            _mockCache.Verify(c => c.SetAsync($"account:{accountId}:{ownerId}", It.IsAny<AccountDto>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenAccountNotOwnedOrMissing()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var ownerId = Guid.NewGuid().ToString();

            _mockCache.Setup(c => c.GetAsync<AccountDto>($"account:{accountId}:{ownerId}"))
                      .ReturnsAsync((AccountDto)null);

            _mockRepo.Setup(r => r.GetByIdAndOwnerIdAsync(accountId, ownerId))
                     .ReturnsAsync((Account)null);

            var query = new GetAccountDetailsQuery(accountId) { InitiatingUserId = ownerId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
