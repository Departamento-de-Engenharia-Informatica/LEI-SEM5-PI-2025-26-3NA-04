
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using APDL.API.Domain.Storage;
using APDL.API.Domain.Storage.ValueObjects;
using APDL.API.Domain.Dock;
using APDL.API.Domain.Shared;

namespace APDL.Tests.Domain.Storage
{
    public class FacilityServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IFacilityRepository> _mockRepo;
        private readonly FacilityService _service;

        public FacilityServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IFacilityRepository>();
            _service = new FacilityService(_mockUnitOfWork.Object, _mockRepo.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldCreateFacility_WhenValidDto()
        {
            var dto = new CreatingFacilityDto
            {
                Type = "Warehouse",
                Location = "Zone A",
                MaxCapacityTEU = 1000,
                DockAssignments = new List<DockAssignmentDto>
                {
                    new DockAssignmentDto
                    {
                        DockId = Guid.NewGuid(),
                        DistanceMeters = 150
                    }
                }
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Facility>())).ReturnsAsync((Facility f) => f);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var result = await _service.AddAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Warehouse", result.Type);
            Assert.Equal("Zone A", result.Location);
            Assert.Equal(1000, result.MaxCapacityTEU);
            Assert.Single(result.DockAssignments);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenTypeIsInvalid()
        {
            var dto = new CreatingFacilityDto
            {
                Type = "InvalidType",
                Location = "Zone X",
                MaxCapacityTEU = 500
            };

            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_ShouldChangeOccupancy_WhenValid()
        {
            var facility = new Warehouse(new Location("Zone B"), new TEUCapacity(1000));
            
            var mockFacilityDto = new FacilityDto(
                facility.Id.AsGuid(),
                "Warehouse",
                "Zone B",
                1000,
                0,
                new List<DockAssignmentDto>()
            );


            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<FacilityId>())).ReturnsAsync(facility);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var result = await _service.UpdateAsync(500, mockFacilityDto);

            Assert.NotNull(result);
            Assert.Equal(500, result.CurrentOccupancyTEU);
        }
    }
}
