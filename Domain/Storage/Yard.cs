
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Storage
{
    public class Yard : Facility
    {
        public Yard(string location, int maxCapacityTEU)
            : base("Yard", location, maxCapacityTEU) { }
    }
}
