    using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.DTO;
using APDL.API.Domain.ContainerAggregate.Repos;
using APDL.API.Domain.ContainerAggregate.ValueObjects;

namespace APDL.Tests.Domain.ContainerAggregate
{
    public class ContainerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IContainerRepository> _mockRepo;
        private readonly ContainerService _service;

        public ContainerServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IContainerRepository>();
            _service = new ContainerService(_mockUnitOfWork.Object, _mockRepo.Object);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfContainers()
        {
            // Arrange
            var containers = CreateTestContainers();
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(containers);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("MSCU1234566", result[0].ContainerNumber);
            Assert.Equal("Reefer", result[0].CargoType);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoContainers()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Container>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnContainer_WhenExists()
        {
            // Arrange
            var container = CreateTestContainers()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(container.Id)).ReturnsAsync(container);

            // Act
            var result = await _service.GetByIdAsync(container.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(container.Id.AsGuid(), result.Id);
            Assert.Equal(container.ContainerNumber.Value, result.ContainerNumber);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var containerId = new ContainerId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(containerId)).ReturnsAsync((Container)null);

            // Act
            var result = await _service.GetByIdAsync(containerId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetByContainerNumberAsync Tests

        [Fact]
        public async Task GetByContainerNumberAsync_ShouldReturnContainer_WhenExists()
        {
            // Arrange
            var container = CreateTestContainers()[0];
            _mockRepo.Setup(r => r.GetByContainerNumberAsync("MSCU1234566")).ReturnsAsync(container);

            // Act
            var result = await _service.GetByContainerNumberAsync("MSCU1234566");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MSCU1234566", result.ContainerNumber);
        }

        [Fact]
        public async Task GetByContainerNumberAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByContainerNumberAsync("INVALID123")).ReturnsAsync((Container)null);

            // Act
            var result = await _service.GetByContainerNumberAsync("INVALID123");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldCreateContainer_WhenValidAndUnique()
        {
            // Arrange
            var dto = new CreateContainerDto
            {
                ContainerNumber = "MSCU1234566",
                CargoType = "General",
                Description = "Electronics",
                SpecialRequirements = "Handle with care",
                Bay = 10,
                Row = 5,
                Tier = 3
            };

            _mockRepo.Setup(r => r.ContainerNumberExistsAsync("MSCU1234566")).ReturnsAsync(false);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Container>())).ReturnsAsync((Container c) => c);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MSCU1234566", result.ContainerNumber);
            Assert.Equal(dto.CargoType, result.CargoType);
            Assert.Equal(dto.Bay, result.Bay);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Container>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenContainerNumberAlreadyExists()
        {
            // Arrange
            var dto = new CreateContainerDto
            {
                ContainerNumber = "MSCU1234566",
                CargoType = "General",
                Description = "Test",
                SpecialRequirements = "",
                Bay = 10,
                Row = 5,
                Tier = 3
            };

            _mockRepo.Setup(r => r.ContainerNumberExistsAsync("MSCU1234566")).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Container>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldUpdateContainer_WhenExists()
        {
            // Arrange
            var container = CreateTestContainers()[0];
            var dto = new UpdateContainerDto
            {
                Id = container.Id.AsGuid(),
                CargoType = "HAZMAT",
                Description = "Updated description",
                SpecialRequirements = "Updated requirements",
                Bay = 20,
                Row = 10,
                Tier = 6
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<ContainerId>())).ReturnsAsync(container);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.CargoType, result.CargoType);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.Bay, result.Bay);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var dto = new UpdateContainerDto
            {
                Id = Guid.NewGuid(),
                CargoType = "General",
                Description = "Test",
                SpecialRequirements = "",
                Bay = 10,
                Row = 5,
                Tier = 3
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<ContainerId>())).ReturnsAsync((Container)null);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldRemoveContainer_WhenExists()
        {
            // Arrange
            var container = CreateTestContainers()[0];

            _mockRepo.Setup(r => r.GetByIdAsync(container.Id)).ReturnsAsync(container);
            _mockRepo.Setup(r => r.Remove(container)).Verifiable();
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(container.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(container.Id.AsGuid(), result.Id);
            _mockRepo.Verify(r => r.Remove(container), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var containerId = new ContainerId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(containerId)).ReturnsAsync((Container)null);

            // Act
            var result = await _service.DeleteAsync(containerId);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<Container>()), Times.Never);
        }

        #endregion

        #region Helper Methods

        private List<Container> CreateTestContainers()
        {
            var container1 = Container.Create(
                "MSCU1234566",
                "Reefer",
                "Refrigerated goods at -18C",
                "Temperature controlled",
                10, 5, 3
            );

            var container2 = Container.Create(
                "CSQU3054383",
                "HAZMAT",
                "Hazardous materials",
                "Handle with extreme care",
                15, 8, 4
            );

            return new List<Container> { container1, container2 };
        }

        #endregion
    }
}