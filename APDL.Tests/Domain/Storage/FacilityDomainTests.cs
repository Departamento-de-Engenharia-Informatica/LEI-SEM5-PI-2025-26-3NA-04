
using System;
using System.Linq;
using Xunit;
using APDL.API.Domain.Storage;
using APDL.API.Domain.Storage.ValueObjects;
using APDL.API.Domain.Dock;
using APDL.API.Domain.Shared;

namespace APDL.Tests.Domain.Storage
{
    public class FacilityDomainTests
    {
        [Fact]
        public void CreatingWarehouse_ShouldSucceed()
        {
            var location = new Location("Zone A");
            var capacity = new TEUCapacity(1000);
            var warehouse = new Warehouse(location, capacity);

            Assert.NotNull(warehouse);
            Assert.Equal("Zone A", warehouse.Location.Value);
            Assert.Equal(1000, warehouse.MaxCapacityTEU.Value);
            Assert.Equal(0, warehouse.CurrentOccupancyTEU);
        }

        [Fact]
        public void CreatingYard_ShouldSucceed()
        {
            var location = new Location("Zone B");
            var capacity = new TEUCapacity(500);
            var yard = new Yard(location, capacity);

            Assert.NotNull(yard);
            Assert.Equal("Zone B", yard.Location.Value);
            Assert.Equal(500, yard.MaxCapacityTEU.Value);
        }

        [Fact]
        public void ChangingOccupancy_AboveMax_ShouldThrow()
        {
            var yard = new Yard(new Location("Zone C"), new TEUCapacity(300));

            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                yard.ChangeCurrentOccupancyTEU(400);
            });

            Assert.Contains("Current occupancy cannot exceed max capacity", ex.Message);
        }

        [Fact]
        public void ChangingOccupancy_WithinLimit_ShouldSucceed()
        {
            var warehouse = new Warehouse(new Location("Zone D"), new TEUCapacity(800));
            warehouse.ChangeCurrentOccupancyTEU(600);

            Assert.Equal(600, warehouse.CurrentOccupancyTEU);
        }

        [Fact]
        public void AssignDock_ShouldAddDockAssignment()
        {
            var yard = new Yard(new Location("Zone E"), new TEUCapacity(1000));
            var dockId = new DockId(Guid.NewGuid());
            var distance = new Distance(150);

            yard.AssignDock(dockId, distance);

            Assert.Single(yard.DockAssignments);
            Assert.Equal(dockId, yard.DockAssignments.First().DockId);
            Assert.Equal(150, yard.DockAssignments.First().DistanceToDock.Meters);
        }
    }
}
