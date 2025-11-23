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
    public class ShippingAgentServiceTests : IDisposable
    {
        private readonly DDDSample1DbContext _context;
        private readonly ShippingAgentService _service;
        private readonly IUnitOfWork _unitOfWork;

        public ShippingAgentServiceTests()
        {
            var options = new DbContextOptionsBuilder<DDDSample1DbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;

            _context = new DDDSample1DbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            var agentRepo = new ShippingAgentRepository(_context);
            var repRepo = new ShippingAgentRepresentativeRepository(_context);
            _service = new ShippingAgentService(_unitOfWork, agentRepo, repRepo);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Create Tests

        [Fact]
        public async Task AddAsync_WithValidData_ShouldCreateShippingAgent()
        {
            // Arrange
            var dto = new CreateShippingAgentDto
            {
                LegalName = "Maersk Line",
                AlternativeName = "Maersk",
                Address = "123 Harbor Street, Copenhagen",
                TaxNumber = "123456789",
                Representatives = new List<CreateRepresentativeDto>
                {
                    new CreateRepresentativeDto
                    {
                        Name = "John Doe",
                        CitizenId = "12345678",
                        Nationality = "Danish",
                        Email = "john@maersk.com",
                        Phone = "912345678",
                    },
                },
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Maersk Line", result.LegalName);
            Assert.Equal("Maersk", result.AlternativeName);
            Assert.Equal("123 Harbor Street, Copenhagen", result.Address);
            Assert.Equal("123456789", result.TaxNumber);
            Assert.Single(result.Representatives);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var dto = new CreateShippingAgentDto
            {
                LegalName = "MSC Mediterranean Shipping",
                AlternativeName = "MSC",
                Address = "456 Port Avenue, Geneva",
                TaxNumber = "987654321",
                Representatives = new List<CreateRepresentativeDto>
                {
                    new CreateRepresentativeDto
                    {
                        Name = "Jane Smith",
                        CitizenId = "87654321",
                        Nationality = "Swiss",
                        Email = "jane@msc.com",
                        Phone = "923456789",
                    },
                },
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            var dbAgent = await _context
                .ShippingAgents.Include(a => a.Representatives)
                .FirstOrDefaultAsync(a => a.Id.AsGuid() == result.Id);
            Assert.NotNull(dbAgent);
            Assert.Equal("MSC Mediterranean Shipping", dbAgent.LegalName.Value);
            Assert.Single(dbAgent.Representatives);
        }

        [Fact]
        public async Task AddAsync_WithNoRepresentatives_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateShippingAgentDto
            {
                LegalName = "Test Company",
                Address = "Test Address",
                TaxNumber = "111222333",
                Representatives = new List<CreateRepresentativeDto>(),
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WithMultipleRepresentatives_ShouldCreateAll()
        {
            // Arrange
            var dto = new CreateShippingAgentDto
            {
                LegalName = "CMA CGM",
                Address = "789 Ocean Road, Marseille",
                TaxNumber = "444555666",
                Representatives = new List<CreateRepresentativeDto>
                {
                    new CreateRepresentativeDto
                    {
                        Name = "Rep One",
                        CitizenId = "11111111",
                        Nationality = "French",
                        Email = "rep1@cmacgm.com",
                        Phone = "911111111",
                    },
                    new CreateRepresentativeDto
                    {
                        Name = "Rep Two",
                        CitizenId = "22222222",
                        Nationality = "French",
                        Email = "rep2@cmacgm.com",
                        Phone = "922222222",
                    },
                },
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.Equal(2, result.Representatives.Count);
        }

        #endregion

        #region Read Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllShippingAgents()
        {
            // Arrange
            await CreateTestAgent("Agent 1", "123456789");
            await CreateTestAgent("Agent 2", "987654321");

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnAgent()
        {
            // Arrange
            var created = await CreateTestAgent("Test Agent", "123456789");

            // Act
            var result = await _service.GetByIdAsync(new ShippingAgentId(created.Id));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal("Test Agent", result.LegalName);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(new ShippingAgentId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateAgent()
        {
            // Arrange
            var agent = await CreateTestAgent("Original Name", "123456789");
            var updateDto = new ShippingAgentDto
            {
                Id = agent.Id,
                LegalName = "Updated Name",
                AlternativeName = "Updated Alt",
                Address = "Updated Address",
                TaxNumber = "999888777",
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.LegalName);
            Assert.Equal("Updated Alt", result.AlternativeName);
            Assert.Equal("Updated Address", result.Address);
            Assert.Equal("999888777", result.TaxNumber);
        }

        [Fact]
        public async Task UpdateAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var agent = await CreateTestAgent("Test Agent", "123456789");
            var updateDto = new ShippingAgentDto
            {
                Id = agent.Id,
                LegalName = "Persisted Update",
                AlternativeName = "Alt Pesist Update",
                Address = "New Address",
                TaxNumber = "555666777",
            };

            // Act
            await _service.UpdateAsync(updateDto);

            // Assert
            var dbAgent = await _context.ShippingAgents.FirstOrDefaultAsync(a =>
                a.Id.AsGuid() == agent.Id
            );
            Assert.NotNull(dbAgent);
            Assert.Equal("Persisted Update", dbAgent.LegalName.Value);
            Assert.Equal("New Address", dbAgent.Address.Value);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotExists_ShouldReturnNull()
        {
            // Arrange
            var updateDto = new ShippingAgentDto
            {
                Id = Guid.NewGuid(),
                LegalName = "Test",
                Address = "Test",
                TaxNumber = "123456789",
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_WhenExists_ShouldDeleteAgent()
        {
            // Arrange
            var agent = await CreateTestAgent("To Delete", "123456789");

            // Act
            var result = await _service.DeleteAsync(new ShippingAgentId(agent.Id));

            // Assert
            Assert.NotNull(result);
            var dbAgent = await _context.ShippingAgents.FirstOrDefaultAsync(a =>
                a.Id.AsGuid() == agent.Id
            );
            Assert.Null(dbAgent);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.DeleteAsync(new ShippingAgentId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddRepresentativeToAgent Tests

        [Fact]
        public async Task AddRepresentativeToAgent_WithValidData_ShouldAddRepresentative()
        {
            // Arrange
            var agent = await CreateTestAgent("Test Agent", "123456789");

            // Create a standalone representative
            var repDto = new CreateRepresentativeDto
            {
                Name = "New Rep",
                CitizenId = "99999999",
                Nationality = "Portuguese",
                Email = "newrep@test.com",
                Phone = "999999999",
            };
            var repService = new ShippingAgentRepresentativeService(
                _unitOfWork,
                new ShippingAgentRepresentativeRepository(_context)
            );
            var createdRep = await repService.AddAsync(repDto);

            // Act
            var result = await _service.AddRepresentativeToAgentAsync(agent.Id, createdRep.Email);

            // Assert
            Assert.NotNull(result);
            var updatedAgent = await _service.GetByIdAsync(new ShippingAgentId(agent.Id));
            Assert.Equal(2, updatedAgent.Representatives.Count);
        }

        [Fact]
        public async Task AddRepresentativeToAgent_WithNonExistentAgent_ShouldThrowException()
        {
            // Arrange
            var repDto = new CreateRepresentativeDto
            {
                Name = "Test Rep",
                CitizenId = "12345678",
                Nationality = "Portuguese",
                Email = "test@test.com",
                Phone = "912345678",
            };
            var repService = new ShippingAgentRepresentativeService(
                _unitOfWork,
                new ShippingAgentRepresentativeRepository(_context)
            );
            var rep = await repService.AddAsync(repDto);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.AddRepresentativeToAgentAsync(Guid.NewGuid(), rep.Email)
            );
        }

        #endregion

        #region Helper Methods

        private async Task<ShippingAgentDto> CreateTestAgent(string legalName, string taxNumber)
        {
            var dto = new CreateShippingAgentDto
            {
                LegalName = legalName,
                AlternativeName = $"{legalName} Alt",
                Address = "Test Address",
                TaxNumber = taxNumber,
                Representatives = new List<CreateRepresentativeDto>
                {
                    new CreateRepresentativeDto
                    {
                        Name = "Test Rep",
                        CitizenId = "12345678",
                        Nationality = "Portuguese",
                        Email = $"{legalName.Replace(" ", "")}@test.com",
                        Phone = "912345678",
                    },
                },
            };

            return await _service.AddAsync(dto);
        }

        #endregion
    }
}
