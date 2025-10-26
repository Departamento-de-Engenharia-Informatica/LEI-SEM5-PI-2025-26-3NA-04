
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Shared;

namespace APDL.Tests.Domain.VesselTypes
{
    public class VesselTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IVesselTypeRepository> _mockRepo = new();
        private readonly VesselTypeService _service;

        public VesselTypeServiceTests()
        {
            _service = new VesselTypeService(_mockUnitOfWork.Object, _mockRepo.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnDto_WhenValid()
        {
           
            var dto = new CreatingVesselTypeDto
            {
                Name = "Feeder",
                Description = "General cargo vessel",
                Capacity = 1000,
                MaxRows = 10,
                MaxBays = 20,
                MaxTiers = 30
            };

            
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselTypeId>()))
                    .ReturnsAsync(new VesselType("Feeder", "General cargo vessel", 1000, 10, 20, 30));

            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var result = await _service.AddAsync(dto);

            Assert.Equal("Feeder", result.Name);
            Assert.Equal("General cargo vessel", result.Description);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedDto()
        {
            var id = Guid.NewGuid().ToString();
            var vesselType = new VesselType("Feeder", "General cargo vessel", 1000, 10, 20, 30);
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselTypeId>())).ReturnsAsync(vesselType);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var dto = new CreatingVesselTypeDto
            {
                Name = "Panamax",
                Description = "General cargo vessel",
                Capacity = 1000,
                MaxRows = 10,
                MaxBays = 20,
                MaxTiers = 30
            };
            var result = await _service.UpdateAsync(id, dto);

            Assert.Equal("Panamax", result.Name);
            Assert.Equal("General cargo vessel", result.Description);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnDeletedDto()
        {
            var id = Guid.NewGuid().ToString();
            var vesselType = new VesselType("Feeder", "General cargo vessel", 1000, 10, 20, 30);
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselTypeId>())).ReturnsAsync(vesselType);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var result = await _service.DeleteAsync(id);

            Assert.Equal("Feeder", result.Name);
        }

        [Fact]
        public async Task SearchAsyncName_ShouldReturnMatchingResults()
        {
            var list = new List<VesselType> {
                new VesselType("Feeder", "General cargo vessel", 1000, 10, 20, 30)
            };
            _mockRepo.Setup(r => r.GetByNameAsync("Feeder")).ReturnsAsync(list);

            var result = await _service.SearchAsyncName("Feeder");

            Assert.Single(result);
            Assert.Equal("Feeder", result[0].Name);
        }

        [Fact]
        public async Task SearchAsyncDescription_ShouldReturnMatchingResults()
        {
            var list = new List<VesselType> {
                new VesselType("Feeder", "General cargo vessel", 1000, 10, 20, 30)
            };
            _mockRepo.Setup(r => r.GetByDescriptionAsync("General cargo vessel")).ReturnsAsync(list);

            var result = await _service.SearchAsyncDescription("General cargo vessel");

            Assert.Single(result);
            Assert.Equal("General cargo vessel", result[0].Description);
        }
    }
}
