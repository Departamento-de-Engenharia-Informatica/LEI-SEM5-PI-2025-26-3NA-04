using APDL.API.Domain.Storage.ValueObjects;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Storage
{
    public class Warehouse : Facility
    {
        protected Warehouse() { }
        public Warehouse(Location location, TEUCapacity maxCapacityTEU)
            : base("Warehouse", location, maxCapacityTEU)
        {
        }
    }
}
