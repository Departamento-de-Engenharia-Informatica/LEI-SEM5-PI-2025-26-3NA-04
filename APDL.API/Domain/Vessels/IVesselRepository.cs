using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Vessels;


namespace APDL.API.Domain.Vessels
{
    public interface IVesselRepository : IRepository<Vessel, VesselId>
    {
        Task<Vessel> GetByImoAsync(string imoNumber);
        Task<List<Vessel>> SearchByNameAsync(string name);
        Task<List<Vessel>> SearchByOperatorAsync(string operatorName);
    }
}