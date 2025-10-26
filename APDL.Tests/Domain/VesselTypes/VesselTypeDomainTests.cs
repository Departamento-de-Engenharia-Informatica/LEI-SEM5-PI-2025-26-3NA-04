
using System;
using Xunit;
using APDL.API.Domain.VesselTypes;

namespace APDL.Tests.Domain.VesselTypes
{
    public class VesselTypeDomainTests
    {
        [Fact]
        public void Constructor_ShouldInitializeAllProperties()
        {
            var vesselType = new VesselType("Feeder", "General cargo vessel", 1000, 10, 20, 30);

            Assert.Equal("Feeder", vesselType.Name.Value);
            Assert.Equal("General cargo vessel", vesselType.Description.Value);
            Assert.Equal(1000, vesselType.Capacity.Value);
            Assert.Equal(10, vesselType.MaxRows.Value);
            Assert.Equal(20, vesselType.MaxBays.Value);
            Assert.Equal(30, vesselType.MaxTiers.Value);
        }

        [Fact]
        public void Update_ShouldModifyAllProperties()
        {
            var vesselType = new VesselType("Feeder", "General cargo vessel", 1000, 10, 20, 30);
            vesselType.Update("Panamax", "General cargo vessel", 2000, 15, 25, 35);

            Assert.Equal("Panamax", vesselType.Name.Value);
            Assert.Equal("General cargo vessel", vesselType.Description.Value);
            Assert.Equal(2000, vesselType.Capacity.Value);
            Assert.Equal(15, vesselType.MaxRows.Value);
            Assert.Equal(25, vesselType.MaxBays.Value);
            Assert.Equal(35, vesselType.MaxTiers.Value);
        }
    }
}
