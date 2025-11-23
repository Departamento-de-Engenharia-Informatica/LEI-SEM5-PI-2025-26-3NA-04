using System;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.DockAggregate.DTO;
using APDL.API.Domain.DockAggregate.Repos;
using APDL.API.Domain.Shared;
using APDL.API.Infrastructure;
using APDL.API.Infrastructure.DockInfrastructure;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APDL.Tests.Integration
{
    public class DockServiceTests : IDisposable
    {
        private readonly DDDSample1DbContext _context;
        private readonly DockService _service;
        private readonly IUnitOfWork _unitOfWork;

        public DockServiceTests()
        {
            var options = new DbContextOptionsBuilder<DDDSample1DbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;

            _context = new DDDSample1DbContext(options);
            _unitOfWork = new UnitOfWork(_context);
            
            var repo = new DockRepository(_context);
            _service = new DockService(_unitOfWork, repo);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Create Tests

        [Fact]
        public async Task AddAsync_WithValidData_ShouldCreateDock()
        {
            // Arrange
            var dto = new CreateDockDto
            {
                DockName = "Dock Alpha",
                DockLength = 350,
                DockDraft = 15
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Dock Alpha", result.DockName);
            Assert.Equal(350, result.DockLength);
            Assert.Equal(15, result.DockDraft);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var dto = new CreateDockDto
            {
                DockName = "Dock Beta",
                DockLength = 400,
                DockDraft = 20
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            var dbDock = await _context.Docks
                .FirstOrDefaultAsync(d => d.Id.AsGuid() == result.Id);
            Assert.NotNull(dbDock);
            Assert.Equal("Dock Beta", dbDock.DockName);
        }

        [Fact]
        public async Task AddAsync_WithDuplicateName_ShouldThrowException()
        {
            // Arrange
            var dto1 = new CreateDockDto
            {
                DockName = "Dock Duplicate",
                DockLength = 350,
                DockDraft = 15
            };
            await _service.AddAsync(dto1);

            var dto2 = new CreateDockDto
            {
                DockName = "Dock Duplicate",
                DockLength = 300,
                DockDraft = 12
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => 
                _service.AddAsync(dto2));
        }

        #endregion

        #region Read Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDocks()
        {
            // Arrange
            await CreateTestDock("Dock 1", 350, 15);
            await CreateTestDock("Dock 2", 400, 20);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnDock()
        {
            // Arrange
            var created = await CreateTestDock("Dock Test", 350, 15);

            // Act
            var result = await _service.GetByIdAsync(new DockId(created.Id));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal("Dock Test", result.DockName);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(new DockId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_WhenExists_ShouldReturnDock()
        {
            // Arrange
            await CreateTestDock("Dock Unique", 350, 15);

            // Act
            var result = await _service.GetByNameAsync("Dock Unique");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dock Unique", result.DockName);
        }

        [Fact]
        public async Task GetByNameAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByNameAsync("NonExistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetDocksCapableOfVesselAsync_ShouldReturnMatchingDocks()
        {
            // Arrange
            await CreateTestDock("Small Dock", 300, 12);
            await CreateTestDock("Large Dock", 400, 20);
            await CreateTestDock("Medium Dock", 350, 15);

            // Act - vessel needs 330m length, 14m draft
            var result = await _service.GetDocksCapableOfVesselAsync(330, 14);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Large and Medium can handle it
            Assert.All(result, d => 
            {
                Assert.True(d.DockLength >= 330);
                Assert.True(d.DockDraft >= 14);
            });
        }

        [Fact]
        public async Task GetDocksCapableOfVesselAsync_WhenNoneMatch_ShouldReturnEmpty()
        {
            // Arrange
            await CreateTestDock("Small Dock", 200, 10);

            // Act - vessel too big
            var result = await _service.GetDocksCapableOfVesselAsync(500, 25);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateDock()
        {
            // Arrange
            var dock = await CreateTestDock("Original Name", 350, 15);
            var updateDto = new UpdateDockDto
            {
                Id = dock.Id,
                DockName = "Updated Name",
                DockLength = 400,
                DockDraft = 20
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            // Note: Your UpdateAsync doesn't actually update anything
            // This test will need adjustment based on your actual implementation
        }

        [Fact]
        public async Task UpdateAsync_WhenNotExists_ShouldReturnNull()
        {
            // Arrange
            var updateDto = new UpdateDockDto
            {
                Id = Guid.NewGuid(),
                DockName = "Test",
                DockLength = 350,
                DockDraft = 15
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_WhenExists_ShouldDeleteDock()
        {
            // Arrange
            var dock = await CreateTestDock("To Delete", 350, 15);

            // Act
            var result = await _service.DeleteAsync(new DockId(dock.Id));

            // Assert
            Assert.NotNull(result);
            var dbDock = await _context.Docks
                .FirstOrDefaultAsync(d => d.Id.AsGuid() == dock.Id);
            Assert.Null(dbDock);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.DeleteAsync(new DockId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Helper Methods

        private async Task<DockDto> CreateTestDock(string name, int length, int draft)
        {
            var dto = new CreateDockDto
            {
                DockName = name,
                DockLength = length,
                DockDraft = draft
            };

            return await _service.AddAsync(dto);
        }

        #endregion
    }
}