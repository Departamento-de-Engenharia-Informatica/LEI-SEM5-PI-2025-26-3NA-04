
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Shared;

namespace APDL.Tests.Domain.Vessels
{
    public class VesselServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IVesselRepository> _mockRepo = new();
        private readonly Mock<IVesselTypeRepository> _mockTypeRepo = new();
        private readonly VesselService _service;

        public VesselServiceTests()
        {
            _service = new VesselService(_mockUnitOfWork.Object, _mockRepo.Object, _mockTypeRepo.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldCreateVessel_WhenValid()
        {
            var typeId = Guid.NewGuid();
            var dto = new CreatingVesselDto
            {
                Name = "Evergreen",
                ImoNumber = "1234567",
                VesselTypeId = typeId,
                Operator = "Maersk"
            };

            _mockTypeRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselTypeId>())).ReturnsAsync(new VesselType("Feeder", "Feeder vessel", 500, 5, 10, 15));
            
            _mockRepo.Setup(r => r.GetByImoAsync("1234567"))
                .ReturnsAsync(new Vessel("Evergreen", "1234567", new VesselTypeId(Guid.NewGuid()), new VesselType("Feeder", "Feeder vessel", 500, 5, 10, 15), "Maersk"));

            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var result = await _service.AddAsync(dto);

            Assert.Equal("Evergreen", result.Name);
            Assert.Equal("1234567", result.ImoNumber);
            Assert.Equal("Maersk", result.Operator);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnDeletedVessel()
        {
            var vessel = new Vessel("Evergreen", "1234567", new VesselTypeId(Guid.NewGuid()), new VesselType("Feeder", "Feeder vessel", 500, 5, 10, 15), "Maersk");
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselId>())).ReturnsAsync(vessel);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var result = await _service.DeleteAsync(vessel.Id);

            Assert.Equal("Evergreen", result.Name);
        }

        [Fact]
        public async Task SearchByNameAsync_ShouldReturnMatchingResults()
        {
            var vessels = new List<Vessel> {
                new Vessel("Evergreen", "1234567", new VesselTypeId(Guid.NewGuid()), new VesselType("Feeder", "Feeder vessel", 500, 5, 10, 15), "Maersk")
            };
            _mockRepo.Setup(r => r.SearchByNameAsync("Evergreen")).ReturnsAsync(vessels);

            var result = await _service.SearchByNameAsync("Evergreen");

            Assert.Single(result);
            Assert.Equal("Evergreen", result[0].Name);
        }

        [Fact]
        public async Task SearchByOperatorAsync_ShouldReturnMatchingResults()
        {
            var vessels = new List<Vessel> {
                new Vessel("Evergreen", "1234567", new VesselTypeId(Guid.NewGuid()), new VesselType("Feeder", "Feeder vessel", 500, 5, 10, 15), "Maersk")
            };
            _mockRepo.Setup(r => r.SearchByOperatorAsync("Maersk")).ReturnsAsync(vessels);

            var result = await _service.SearchByOperatorAsync("Maersk");

            Assert.Single(result);
            Assert.Equal("Maersk", result[0].Operator);
        }

        [Fact]
        public async Task GetByImoAsync_ShouldReturnVessel()
        {
            var vessel = new Vessel("Evergreen", "1234567", new VesselTypeId(Guid.NewGuid()), new VesselType("Feeder", "Feeder vessel", 500, 5, 10, 15), "Maersk");
            _mockRepo.Setup(r => r.GetByImoAsync("1234567")).ReturnsAsync(vessel);

            var result = await _service.GetByImoAsync("1234567");

            Assert.Equal("Evergreen", result.Name);
        }
    }
}
