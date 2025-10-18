
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Storage
{
    public class Warehouse : Facility
    {
        public Warehouse(string location, int maxCapacityTEU)
            : base("Warehouse", location, maxCapacityTEU) { }
    }
}
