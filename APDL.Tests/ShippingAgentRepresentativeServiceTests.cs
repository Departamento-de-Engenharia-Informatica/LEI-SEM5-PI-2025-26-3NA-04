using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.DTO;
using APDL.API.Infrastructure;
using APDL.API.Infrastructure.Shared;
using APDL.API.Infrastructure.ShippingAgentInfrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APDL.Tests.Integration
{
    public class ShippingAgentRepresentativeServiceTests : IDisposable
    {
        private readonly DDDSample1DbContext _context;
        private readonly ShippingAgentRepresentativeService _service;
        private readonly IUnitOfWork _unitOfWork;

        public ShippingAgentRepresentativeServiceTests()
        {
            var options = new DbContextOptionsBuilder<DDDSample1DbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;

            _context = new DDDSample1DbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            var repo = new ShippingAgentRepresentativeRepository(_context);
            _service = new ShippingAgentRepresentativeService(_unitOfWork, repo);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Create Tests

        [Fact]
        public async Task AddAsync_WithValidData_ShouldCreateRepresentative()
        {
            // Arrange
            var dto = new CreateRepresentativeDto
            {
                Name = "John Doe",
                CitizenId = "12345678",
                Nationality = "Portuguese",
                Email = "john@example.com",
                Phone = "912345678",
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("12345678", result.CitizenId);
            Assert.Equal("Portuguese", result.Nationality);
            Assert.Equal("john@example.com", result.Email);
            Assert.Equal("912345678", result.Phone);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var dto = new CreateRepresentativeDto
            {
                Name = "Jane Smith",
                CitizenId = "87654321",
                Nationality = "Spanish",
                Email = "jane@example.com",
                Phone = "923456789",
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            var dbRep = await _context.ShippingAgentRepresentatives.FirstOrDefaultAsync(r =>
                r.Id.AsGuid() == result.Id
            );
            Assert.NotNull(dbRep);
            Assert.Equal("Jane Smith", dbRep.Name.Value);
            Assert.Equal("jane@example.com", dbRep.Email.Value);
        }

        [Fact]
        public async Task AddAsync_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateRepresentativeDto
            {
                Name = "Test",
                CitizenId = "12345678",
                Nationality = "Portuguese",
                Email = "invalid-email",
                Phone = "912345678",
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WithInvalidPhone_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateRepresentativeDto
            {
                Name = "Test",
                CitizenId = "12345678",
                Nationality = "Portuguese",
                Email = "test@example.com",
                Phone = "812345678",
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WithShortCitizenId_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateRepresentativeDto
            {
                Name = "Test",
                CitizenId = "123",
                Nationality = "Portuguese",
                Email = "test@example.com",
                Phone = "912345678",
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        #endregion

        #region Read Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRepresentatives()
        {
            // Arrange
            await CreateTestRepresentative("Rep 1", "rep1@test.com");
            await CreateTestRepresentative("Rep 2", "rep2@test.com");

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnRepresentative()
        {
            // Arrange
            var created = await CreateTestRepresentative("Test Rep", "test@test.com");

            // Act
            var result = await _service.GetByIdAsync(new ShippingAgentRepresentativeId(created.Id));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal("Test Rep", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(
                new ShippingAgentRepresentativeId(Guid.NewGuid())
            );

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateRepresentative()
        {
            // Arrange
            var rep = await CreateTestRepresentative("Original Name", "original@test.com");
            var updateDto = new RepresentativeDto
            {
                Id = rep.Id,
                Name = "Updated Name",
                CitizenId = "99999999",
                Nationality = "French",
                Email = "updated@test.com",
                Phone = "999999999",
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal("99999999", result.CitizenId);
            Assert.Equal("French", result.Nationality);
            Assert.Equal("updated@test.com", result.Email);
            Assert.Equal("999999999", result.Phone);
        }

        [Fact]
        public async Task UpdateAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var rep = await CreateTestRepresentative("Test Rep", "test@test.com");
            var updateDto = new RepresentativeDto
            {
                Id = rep.Id,
                Name = "Persisted Name",
                CitizenId = "88888888",
                Nationality = "Italian",
                Email = "persisted@test.com",
                Phone = "988888888",
            };

            // Act
            await _service.UpdateAsync(updateDto);

            // Assert
            var dbRep = await _context.ShippingAgentRepresentatives.FirstOrDefaultAsync(r =>
                r.Id.AsGuid() == rep.Id
            );
            Assert.NotNull(dbRep);
            Assert.Equal("Persisted Name", dbRep.Name.Value);
            Assert.Equal("persisted@test.com", dbRep.Email.Value);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotExists_ShouldReturnNull()
        {
            // Arrange
            var updateDto = new RepresentativeDto
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                CitizenId = "12345678",
                Nationality = "Portuguese",
                Email = "test@test.com",
                Phone = "912345678",
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_WhenExists_ShouldDeleteRepresentative()
        {
            // Arrange
            var rep = await CreateTestRepresentative("To Delete", "delete@test.com");

            // Act
            var result = await _service.DeleteAsync(new ShippingAgentRepresentativeId(rep.Id));

            // Assert
            Assert.NotNull(result);
            var dbRep = await _context.ShippingAgentRepresentatives.FirstOrDefaultAsync(r =>
                r.Id.AsGuid() == rep.Id
            );
            Assert.Null(dbRep);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.DeleteAsync(
                new ShippingAgentRepresentativeId(Guid.NewGuid())
            );

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Activation Tests

        [Fact]
        public async Task DeactivateAsync_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var rep = await CreateTestRepresentative("Test Rep", "test@test.com");

            // Act
            await _service.DeactivateAsync(new ShippingAgentRepresentativeId(rep.Id));

            // Assert
            var dbRep = await _context.ShippingAgentRepresentatives.FirstOrDefaultAsync(r =>
                r.Id.AsGuid() == rep.Id
            );
            Assert.NotNull(dbRep);
            Assert.False(dbRep.IsActive);
        }

        [Fact]
        public async Task ReactivateAsync_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var rep = await CreateTestRepresentative("Test Rep", "test@test.com");
            await _service.DeactivateAsync(new ShippingAgentRepresentativeId(rep.Id));

            // Act
            await _service.ReactivateAsync(new ShippingAgentRepresentativeId(rep.Id));

            // Assert
            var dbRep = await _context.ShippingAgentRepresentatives.FirstOrDefaultAsync(r =>
                r.Id.AsGuid() == rep.Id
            );
            Assert.NotNull(dbRep);
            Assert.True(dbRep.IsActive);
        }

        [Fact]
        public async Task DeactivateAsync_WhenNotExists_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeactivateAsync(new ShippingAgentRepresentativeId(Guid.NewGuid()))
            );
        }

        [Fact]
        public async Task ReactivateAsync_WhenNotExists_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.ReactivateAsync(new ShippingAgentRepresentativeId(Guid.NewGuid()))
            );
        }

        #endregion

        #region Helper Methods

        private async Task<RepresentativeDto> CreateTestRepresentative(string name, string email)
        {
            var dto = new CreateRepresentativeDto
            {
                Name = name,
                CitizenId = "12345678",
                Nationality = "Portuguese",
                Email = email,
                Phone = "912345678",
            };

            return await _service.AddAsync(dto);
        }

        #endregion
    }
}
