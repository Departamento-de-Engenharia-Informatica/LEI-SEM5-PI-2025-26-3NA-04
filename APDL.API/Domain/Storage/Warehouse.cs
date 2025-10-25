
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Storage
{
    public class Warehouse : Facility
    {
        public Warehouse(string location, int maxCapacityTEU)
            : base("Warehouse", location, maxCapacityTEU) { }
    }
}
