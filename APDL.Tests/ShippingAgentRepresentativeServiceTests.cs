using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.DTO;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using Moq;
using Xunit;

namespace APDL.API.Tests.Domain.ShippingAgentAggregate
{
    public class ShippingAgentRepresentativeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IShippingAgentRepresentativeRepository> _mockRepo;
        private readonly ShippingAgentRepresentativeService _service;

        public ShippingAgentRepresentativeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IShippingAgentRepresentativeRepository>();
            _service = new ShippingAgentRepresentativeService(_mockUnitOfWork.Object, _mockRepo.Object);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfRepresentativeDtos()
        {
            // Arrange
            var representatives = CreateTestRepresentatives();
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(representatives);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John Doe", result[0].Name);
            Assert.Equal("12345678", result[0].CitizenId);
            Assert.True(result[0].IsActive);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoRepresentatives()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ShippingAgentRepresentative>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRepresentativeDto_WhenRepresentativeExists()
        {
            // Arrange
            var representative = CreateTestRepresentatives()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(representative.Id)).ReturnsAsync(representative);

            // Act
            var result = await _service.GetByIdAsync(representative.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(representative.Id.AsGuid(), result.Id);
            Assert.Equal(representative.Name.Value, result.Name);
            Assert.Equal(representative.CitizenId.Value, result.CitizenId);
            Assert.Equal(representative.Nationality.Value, result.Nationality);
            Assert.Equal(representative.Email.Value, result.Email);
            Assert.Equal(representative.Phone.Value, result.Phone);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenRepresentativeDoesNotExist()
        {
            // Arrange
            var repId = new ShippingAgentRepresentativeId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(repId)).ReturnsAsync((ShippingAgentRepresentative)null);

            // Act
            var result = await _service.GetByIdAsync(repId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldCreateAndReturnRepresentative_WhenValidDto()
        {
            // Arrange
            var dto = new CreateRepresentativeDto
            {
                Name = "Jane Smith",
                CitizenId = "87654321",
                Nationality = "US",
                Email = "jane@example.com",
                Phone = "923456789"
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ShippingAgentRepresentative>()))
                .ReturnsAsync((ShippingAgentRepresentative r) => r);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.CitizenId, result.CitizenId);
            Assert.Equal(dto.Nationality, result.Nationality);
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.Phone, result.Phone);
            Assert.True(result.IsActive);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<ShippingAgentRepresentative>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndReturnRepresentative_WhenRepresentativeExists()
        {
            // Arrange
            var representative = CreateTestRepresentatives()[0];
            var dto = new RepresentativeDto
            {
                Id = representative.Id.AsGuid(),
                Name = "Updated Name",
                CitizenId = "99999999",
                Nationality = "UK",
                Email = "updated@example.com",
                Phone = "934567890",
                IsActive = true
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<ShippingAgentRepresentativeId>()))
                .ReturnsAsync(representative);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenRepresentativeDoesNotExist()
        {
            // Arrange
            var dto = new RepresentativeDto
            {
                Id = Guid.NewGuid(),
                Name = "Updated Name",
                CitizenId = "99999999",
                Nationality = "UK",
                Email = "updated@example.com",
                Phone = "934567890"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<ShippingAgentRepresentativeId>()))
                .ReturnsAsync((ShippingAgentRepresentative)null);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAndReturnRepresentative_WhenRepresentativeExists()
        {
            // Arrange
            var representative = CreateTestRepresentatives()[0];
            var repId = representative.Id;

            _mockRepo.Setup(r => r.GetByIdAsync(repId)).ReturnsAsync(representative);
            _mockRepo.Setup(r => r.Remove(representative)).Verifiable();
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(repId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(representative.Id.AsGuid(), result.Id);
            Assert.Equal(representative.Name.Value, result.Name);
            _mockRepo.Verify(r => r.Remove(representative), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNull_WhenRepresentativeDoesNotExist()
        {
            // Arrange
            var repId = new ShippingAgentRepresentativeId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(repId)).ReturnsAsync((ShippingAgentRepresentative)null);

            // Act
            var result = await _service.DeleteAsync(repId);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<ShippingAgentRepresentative>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        #endregion

        #region DeactivateAsync Tests

        [Fact]
        public async Task DeactivateAsync_ShouldDeactivateRepresentative_WhenRepresentativeExists()
        {
            // Arrange
            var representative = CreateTestRepresentatives()[0];
            var repId = representative.Id;

            _mockRepo.Setup(r => r.GetByIdAsync(repId)).ReturnsAsync(representative);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.DeactivateAsync(repId);

            // Assert
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeactivateAsync_ShouldThrowKeyNotFoundException_WhenRepresentativeDoesNotExist()
        {
            // Arrange
            var repId = new ShippingAgentRepresentativeId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(repId)).ReturnsAsync((ShippingAgentRepresentative)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeactivateAsync(repId));
            Assert.Contains("not found", exception.Message);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        #endregion

        #region ReactivateAsync Tests

        [Fact]
        public async Task ReactivateAsync_ShouldReactivateRepresentative_WhenRepresentativeExists()
        {
            // Arrange
            var representative = CreateTestRepresentatives()[0];
            var repId = representative.Id;

            _mockRepo.Setup(r => r.GetByIdAsync(repId)).ReturnsAsync(representative);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.ReactivateAsync(repId);

            // Assert
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ReactivateAsync_ShouldThrowKeyNotFoundException_WhenRepresentativeDoesNotExist()
        {
            // Arrange
            var repId = new ShippingAgentRepresentativeId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(repId)).ReturnsAsync((ShippingAgentRepresentative)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ReactivateAsync(repId));
            Assert.Contains("not found", exception.Message);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        #endregion

        #region Helper Methods

        private List<ShippingAgentRepresentative> CreateTestRepresentatives()
        {
            var rep1 = new ShippingAgentRepresentative(
                new Name("John Doe"),
                new CitizenId("12345678"),
                new Nationality("PT"),
                new Email("john@example.com"),
                new Phone("912345678")
            );

            var rep2 = new ShippingAgentRepresentative(
                new Name("Jane Smith"),
                new CitizenId("87654321"),
                new Nationality("US"),
                new Email("jane@example.com"),
                new Phone("923456789")
            ); 

            return new List<ShippingAgentRepresentative> { rep1, rep2 };
        }

        #endregion
    }
}