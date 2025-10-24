using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetTransactionHistoryQueryTests
    {
        private readonly Mock<IAccountRepository> _mockRepo;
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<GetTransactionHistoryQueryHandler>> _mockLogger;
        private readonly GetTransactionHistoryQueryHandler _handler;

        public GetTransactionHistoryQueryTests()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _mockCache = new Mock<ICacheService>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<GetTransactionHistoryQueryHandler>>();

            _handler = new GetTransactionHistoryQueryHandler(
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

            var txList = new List<TransactionDto>
            {
                new TransactionDto { Id = Guid.NewGuid(), AccountId = accountId, Amount = 100, Type = "Deposit", Timestamp = DateTime.UtcNow, NewBalance = 100 }
            };

            _mockCache.Setup(c => c.GetAsync<IEnumerable<TransactionDto>>($"transactions:{accountId}:{ownerId}"))
                      .ReturnsAsync(txList);

            var query = new GetTransactionHistoryQuery(accountId) { InitiatingUserId = ownerId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            _mockRepo.Verify(r => r.GetTransactionHistoryByAccountIdAndOwnerIdAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldFetchFromDb_AndCache_WhenNotCached()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var ownerId = Guid.NewGuid().ToString();

            _mockCache.Setup(c => c.GetAsync<IEnumerable<TransactionDto>>($"transactions:{accountId}:{ownerId}"))
                      .ReturnsAsync((IEnumerable<TransactionDto>)null);

            var tx = new Transaction(accountId, 50m, Domain.Enums.TransactionType.Deposit, 50m);
            var txListDomain = new List<Transaction> { tx };

            _mockRepo.Setup(r => r.GetTransactionHistoryByAccountIdAndOwnerIdAsync(accountId, ownerId))
                     .ReturnsAsync(txListDomain);

            var query = new GetTransactionHistoryQuery(accountId) { InitiatingUserId = ownerId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            _mockCache.Verify(c => c.SetAsync($"transactions:{accountId}:{ownerId}", It.IsAny<IEnumerable<TransactionDto>>(), TimeSpan.FromMinutes(3)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenNoTransactionsOrNoAccess()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var ownerId = Guid.NewGuid().ToString();

            _mockCache.Setup(c => c.GetAsync<IEnumerable<TransactionDto>>($"transactions:{accountId}:{ownerId}"))
                      .ReturnsAsync((IEnumerable<TransactionDto>)null);

            _mockRepo.Setup(r => r.GetTransactionHistoryByAccountIdAndOwnerIdAsync(accountId, ownerId))
                     .ReturnsAsync((ICollection<Transaction>)null);

            var query = new GetTransactionHistoryQuery(accountId) { InitiatingUserId = ownerId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
