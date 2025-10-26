
using System;
using Xunit;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Shared;

namespace APDL.Tests.Domain.Vessels
{
    public class VesselDomainTests
    {
        [Fact]
        public void Constructor_ShouldInitializeAllProperties()
        {
            var vesselTypeId = new VesselTypeId(Guid.NewGuid());
            var vesselType = new VesselType("Feeder", "Feeder vessel", 500, 5, 10, 15);
            var vessel = new Vessel("Evergreen", "1234567", vesselTypeId, vesselType, "Maersk");

            Assert.Equal("Evergreen", vessel.Name.Value);
            Assert.Equal("1234567", vessel.ImoNumber.Value);
            Assert.Equal(vesselTypeId, vessel.VesselTypeId);
            Assert.Equal("Feeder", vessel.VesselType.Name);
            Assert.Equal("Maersk", vessel.Operator.Value);
        }

        [Fact]
        public void Constructor_WithInvalidImo_ShouldThrow()
        {
            var vesselTypeId = new VesselTypeId(Guid.NewGuid());
            var vesselType = new VesselType("Feeder", "Feeder vessel", 500, 5, 10, 15);
            Assert.Throws<BusinessRuleValidationException>(() => new Vessel("Evergreen", "123456", vesselTypeId, vesselType, "Maersk"));
        }
    }
}
