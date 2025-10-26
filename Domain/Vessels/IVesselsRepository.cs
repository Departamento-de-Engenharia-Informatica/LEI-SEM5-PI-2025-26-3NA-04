using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Vessels;


namespace DDDSample1.Domain.Vessels
{
    public interface IVesselRepository : IRepository<Vessel, VesselId>
    {
        Task<Vessel> GetByImoAsync(string imoNumber);
        Task<List<Vessel>> SearchByNameAsync(string name);
        Task<List<Vessel>> SearchByOperatorAsync(string operatorName);
    }
}
