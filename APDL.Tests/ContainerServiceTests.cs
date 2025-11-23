using System;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.DTO;
using APDL.API.Domain.ContainerAggregate.Repos;
using APDL.API.Domain.Shared;
using APDL.API.Infrastructure;
using APDL.API.Infrastructure.ContainerInfrastructure;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APDL.Tests.Integration
{
    public class ContainerServiceTests : IDisposable
    {
        private readonly DDDSample1DbContext _context;
        private readonly ContainerService _service;
        private readonly IUnitOfWork _unitOfWork;

        public ContainerServiceTests()
        {
            var options = new DbContextOptionsBuilder<DDDSample1DbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;

            _context = new DDDSample1DbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            var repo = new ContainerRepository(_context);
            _service = new ContainerService(_unitOfWork, repo);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Create Tests

        [Fact]
        public async Task AddAsync_WithValidData_ShouldCreateContainer()
        {
            // Arrange
            var dto = new CreateContainerDto
            {
                ContainerNumber = "MSCU1234567",
                CargoType = "Dry Goods",
                Description = "Electronics and appliances",
                SpecialRequirements = "Handle with care",
                Bay = 1,
                Row = 2,
                Tier = 3
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("MSCU1234567", result.ContainerNumber);
            Assert.Equal("Dry Goods", result.CargoType);
            Assert.Equal("Electronics and appliances", result.Description);
            Assert.Equal("Handle with care", result.SpecialRequirements);
            Assert.Equal(1, result.Bay);
            Assert.Equal(2, result.Row);
            Assert.Equal(3, result.Tier);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var dto = new CreateContainerDto
            {
                ContainerNumber = "ABCU9876543",
                CargoType = "Refrigerated",
                Description = "Fresh produce",
                SpecialRequirements = "Keep at -5°C",
                Bay = 5,
                Row = 10,
                Tier = 15
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            var dbContainer = await _context.Containers
                .FirstOrDefaultAsync(c => c.Id.AsGuid() == result.Id);
            Assert.NotNull(dbContainer);
            Assert.Equal("ABCU9876543", dbContainer.ContainerNumber.Value);
            Assert.Equal("Fresh produce", dbContainer.Description.Value);
        }

        [Fact]
        public async Task AddAsync_WithDuplicateContainerNumber_ShouldThrowException()
        {
            // Arrange
            var dto1 = new CreateContainerDto
            {
                ContainerNumber = "MSCU1111111",
                CargoType = "Dry Goods",
                Description = "First container",
                SpecialRequirements = "None",
                Bay = 1,
                Row = 2,
                Tier = 3
            };
            await _service.AddAsync(dto1);

            var dto2 = new CreateContainerDto
            {
                ContainerNumber = "MSCU1111111",
                CargoType = "Dry Goods",
                Description = "Duplicate container",
                SpecialRequirements = "None",
                Bay = 4,
                Row = 5,
                Tier = 6
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.AddAsync(dto2)
            );
        }

        [Fact]
        public async Task AddAsync_WithInvalidContainerNumber_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateContainerDto
            {
                ContainerNumber = "INVALID",
                CargoType = "Dry Goods",
                Description = "Test",
                SpecialRequirements = "None",
                Bay = 1,
                Row = 2,
                Tier = 3
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.AddAsync(dto)
            );
        }

        [Fact]
        public async Task AddAsync_WithInvalidPosition_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateContainerDto
            {
                ContainerNumber = "MSCU2222222",
                CargoType = "Dry Goods",
                Description = "Test",
                SpecialRequirements = "None",
                Bay = 0,
                Row = 2,
                Tier = 3
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.AddAsync(dto)
            );
        }

        #endregion

        #region Read Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllContainers()
        {
            // Arrange
            await CreateTestContainer("MSCU1111111", "Dry Goods", 1, 2, 3);
            await CreateTestContainer("ABCU2222222", "Refrigerated", 4, 5, 6);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnContainer()
        {
            // Arrange
            var created = await CreateTestContainer("MSCU3333333", "Dry Goods", 1, 2, 3);

            // Act
            var result = await _service.GetByIdAsync(new ContainerId(created.Id));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal("MSCU3333333", result.ContainerNumber);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(new ContainerId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByContainerNumberAsync_WhenExists_ShouldReturnContainer()
        {
            // Arrange
            await CreateTestContainer("MSCU4444444", "Dry Goods", 1, 2, 3);

            // Act
            var result = await _service.GetByContainerNumberAsync("MSCU4444444");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MSCU4444444", result.ContainerNumber);
        }

        [Fact]
        public async Task GetByContainerNumberAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByContainerNumberAsync("NONEXIST000");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByContainerNumberAsync_CaseInsensitive_ShouldReturnContainer()
        {
            // Arrange
            await CreateTestContainer("MSCU5555555", "Dry Goods", 1, 2, 3);

            // Act
            var result = await _service.GetByContainerNumberAsync("mscu5555555");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MSCU5555555", result.ContainerNumber);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateContainer()
        {
            // Arrange
            var container = await CreateTestContainer("MSCU6666666", "Dry Goods", 1, 2, 3);
            var updateDto = new UpdateContainerDto
            {
                Id = container.Id,
                CargoType = "Refrigerated",
                Description = "Updated description",
                SpecialRequirements = "Updated requirements",
                Bay = 10,
                Row = 20,
                Tier = 30
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Refrigerated", result.CargoType);
            Assert.Equal("Updated description", result.Description);
            Assert.Equal("Updated requirements", result.SpecialRequirements);
            Assert.Equal(10, result.Bay);
            Assert.Equal(20, result.Row);
            Assert.Equal(30, result.Tier);
        }

        [Fact]
        public async Task UpdateAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var container = await CreateTestContainer("MSCU7777777", "Dry Goods", 1, 2, 3);
            var updateDto = new UpdateContainerDto
            {
                Id = container.Id,
                CargoType = "Hazardous",
                Description = "Chemicals",
                SpecialRequirements = "Special handling required",
                Bay = 5,
                Row = 6,
                Tier = 7
            };

            // Act
            await _service.UpdateAsync(updateDto);

            // Assert
            var dbContainer = await _context.Containers
                .FirstOrDefaultAsync(c => c.Id.AsGuid() == container.Id);
            Assert.NotNull(dbContainer);
            Assert.Equal("Hazardous", dbContainer.CargoType.Value);
            Assert.Equal("Chemicals", dbContainer.Description.Value);
            Assert.Equal(5, dbContainer.Position.Bay);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotExists_ShouldReturnNull()
        {
            // Arrange
            var updateDto = new UpdateContainerDto
            {
                Id = Guid.NewGuid(),
                CargoType = "Test",
                Description = "Test",
                SpecialRequirements = "Test",
                Bay = 1,
                Row = 2,
                Tier = 3
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidCargoType_ShouldThrowException()
        {
            // Arrange
            var container = await CreateTestContainer("MSCU8888888", "Dry Goods", 1, 2, 3);
            var updateDto = new UpdateContainerDto
            {
                Id = container.Id,
                CargoType = "",
                Description = "Test",
                SpecialRequirements = "Test",
                Bay = 1,
                Row = 2,
                Tier = 3
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.UpdateAsync(updateDto)
            );
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidPosition_ShouldThrowException()
        {
            // Arrange
            var container = await CreateTestContainer("MSCU9999999", "Dry Goods", 1, 2, 3);
            var updateDto = new UpdateContainerDto
            {
                Id = container.Id,
                CargoType = "Dry Goods",
                Description = "Test",
                SpecialRequirements = "Test",
                Bay = 0,
                Row = 2,
                Tier = 3
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.UpdateAsync(updateDto)
            );
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_WhenExists_ShouldDeleteContainer()
        {
            // Arrange
            var container = await CreateTestContainer("ABCU1111112", "Dry Goods", 1, 2, 3);

            // Act
            var result = await _service.DeleteAsync(new ContainerId(container.Id));

            // Assert
            Assert.NotNull(result);
            var dbContainer = await _context.Containers
                .FirstOrDefaultAsync(c => c.Id.AsGuid() == container.Id);
            Assert.Null(dbContainer);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.DeleteAsync(new ContainerId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnDeletedContainerData()
        {
            // Arrange
            var container = await CreateTestContainer("ABCU2222223", "Refrigerated", 5, 10, 15);

            // Act
            var result = await _service.DeleteAsync(new ContainerId(container.Id));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ABCU2222223", result.ContainerNumber);
            Assert.Equal("Refrigerated", result.CargoType);
        }

        #endregion

        #region Helper Methods

        private async Task<ContainerDto> CreateTestContainer(
            string containerNumber,
            string cargoType,
            int bay,
            int row,
            int tier
        )
        {
            var dto = new CreateContainerDto
            {
                ContainerNumber = containerNumber,
                CargoType = cargoType,
                Description = "Test description",
                SpecialRequirements = "Test requirements",
                Bay = bay,
                Row = row,
                Tier = tier
            };

            return await _service.AddAsync(dto);
        }

        #endregion
    }
}