
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Storage
{
    public class Yard : Facility
    {
        public Yard(string location, int maxCapacityTEU)
            : base("Yard", location, maxCapacityTEU) { }
    }
}
