using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.DTO;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;
using APDL.API.Domain.ShippingAgentAggregate.Repos;
using APDL.API.Domain.Shared;
using Moq;
using Xunit;

namespace APDL.API.Tests.Domain.ShippingAgentAggregate
{
    public class ShippingAgentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IShippingAgentRepository> _mockRepo;
        private readonly ShippingAgentService _service;

        public ShippingAgentServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IShippingAgentRepository>();
            _service = new ShippingAgentService(_mockUnitOfWork.Object, _mockRepo.Object);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfShippingAgentDtos()
        {
            // Arrange
            var agents = CreateTestShippingAgents();
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(agents);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Agent 1", result[0].LegalName);
            Assert.Single(result[0].Representatives);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoAgents()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ShippingAgent>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnShippingAgentDto_WhenAgentExists()
        {
            // Arrange
            var agent = CreateTestShippingAgents()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(agent.Id)).ReturnsAsync(agent);

            // Act
            var result = await _service.GetByIdAsync(agent.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(agent.Id.AsGuid(), result.Id);
            Assert.Equal(agent.LegalName.Value, result.LegalName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAgentDoesNotExist()
        {
            // Arrange
            var agentId = new ShippingAgentId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(agentId)).ReturnsAsync((ShippingAgent)null);

            // Act
            var result = await _service.GetByIdAsync(agentId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldCreateAndReturnShippingAgent_WhenValidDto()
        {
            // Arrange
            var dto = new CreateShippingAgentDto
            {
                LegalName = "New Agent",
                AlternativeName = "New Alt",
                Address = "123 Test St",
                TaxNumber = "123456789",
                Representatives = new List<CreateRepresentativeDto>
                {
                    new CreateRepresentativeDto
                    {
                        Name = "John Doe",
                        CitizenId = "12345678",
                        Nationality = "US",
                        Email = "john@example.com",
                        Phone = "912345678"
                    }
                }
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ShippingAgent>())).ReturnsAsync((ShippingAgent a) => a);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.LegalName, result.LegalName);
            Assert.Equal(dto.AlternativeName, result.AlternativeName);
            Assert.Single(result.Representatives);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<ShippingAgent>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }


        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenNoRepresentatives()
        {
            // Arrange
            var dto = new CreateShippingAgentDto
            {
                LegalName = "New Agent",
                Address = "123 Test St",
                TaxNumber = "123456789",
                Representatives = new List<CreateRepresentativeDto>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenRepresentativesIsNull()
        {
            // Arrange
            var dto = new CreateShippingAgentDto
            {
                LegalName = "New Agent",
                Address = "123 Test St",
                TaxNumber = "123456789",
                Representatives = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_ShouldHandleNullAlternativeName()
        {
            // Arrange
            var dto = new CreateShippingAgentDto
            {
                LegalName = "New Agent",
                AlternativeName = null,
                Address = "123 Test St",
                TaxNumber = "123456789",
                Representatives = new List<CreateRepresentativeDto>
                {
                    new CreateRepresentativeDto
                    {
                        Name = "John Doe",
                        CitizenId = "12345678",
                        Nationality = "US",
                        Email = "john@example.com",
                        Phone = "923456789"
                    }
                }
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ShippingAgent>())).ReturnsAsync((ShippingAgent a) => a);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.AlternativeName);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndReturnAgent_WhenAgentExists()
        {
            // Arrange
            var representatives = new List<ShippingAgentRepresentative>
            {
                CreateTestRepresentative("Rep 1")
            };
            
            var agent = new ShippingAgent(
                "Original Name",
                "Original Alt",
                "Original Address",
                "123456789",
                representatives
            );

            var dto = new ShippingAgentDto
            {
                Id = agent.Id.AsGuid(),
                LegalName = "Updated Name",
                AlternativeName = "Updated Alt",
                Address = "Updated Address",
                TaxNumber = "987654321"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<ShippingAgentId>())).ReturnsAsync(agent);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.NotNull(result);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenAgentDoesNotExist()
        {
            // Arrange
            var dto = new ShippingAgentDto
            {
                Id = Guid.NewGuid(),
                LegalName = "Updated Name",
                Address = "Updated Address",
                TaxNumber = "987654321"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<ShippingAgentId>())).ReturnsAsync((ShippingAgent)null);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAndReturnAgent_WhenAgentExists()
        {
            // Arrange
            var agent = CreateTestShippingAgents()[0];
            var agentId = agent.Id;

            _mockRepo.Setup(r => r.GetByIdAsync(agentId)).ReturnsAsync(agent);
            _mockRepo.Setup(r => r.Remove(agent)).Verifiable();
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(agentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(agent.Id.AsGuid(), result.Id);
            _mockRepo.Verify(r => r.Remove(agent), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNull_WhenAgentDoesNotExist()
        {
            // Arrange
            var agentId = new ShippingAgentId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(agentId)).ReturnsAsync((ShippingAgent)null);

            // Act
            var result = await _service.DeleteAsync(agentId);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<ShippingAgent>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        #endregion

        #region Helper Methods

        private List<ShippingAgent> CreateTestShippingAgents()
        {
            Console.WriteLine("IN CREATE");

            var agent1 = new ShippingAgent(
                "Test Agent 1",
                "Alt Name 1",
                "Address 1",
                "123456789",
                new List<ShippingAgentRepresentative>
                {
                    CreateTestRepresentative("Rep 1")
                }
            );

            var agent2 = new ShippingAgent(
                "Test Agent 2",
                null,
                "Address 2",
                "987654321",
                new List<ShippingAgentRepresentative>
                {
                    CreateTestRepresentative("Rep 2")
                }
            );

            return new List<ShippingAgent> { agent1, agent2 };
        }

        private ShippingAgentRepresentative CreateTestRepresentative(string name)
        {
            return new ShippingAgentRepresentative(
                new Name(name),
                new CitizenId("12345678"),
                new Nationality("PT"),
                new Email("test@example.com"),
                new Phone("912345678")
            );
        }

        #endregion
    }
}