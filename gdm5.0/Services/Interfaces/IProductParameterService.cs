using gdm5._0.DTO;
using gdm5._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
    public interface IProductParameterService : IBaseServices<ProductParameter>
    {
        Task<IEnumerable<ProductParameter>> GetProductParameters(long id);
        Task<IEnumerable<DiameterDTO>> GetProductDiameters(int id, string param, int paramId);
        Task<IEnumerable<ProductDTO>> GetProductsByDiameter(int typeId, string param, int paramId, int paramDiameterId, string diameter);
        Task<IEnumerable<ProductParameter>> GetSortProductParameters(long id);
        Task<double> GetSumSteelBars(int value);
        Task<double> GetSumTubes(int value);
        Task<double> GetSumQuantityByParam(int typeId, string param, int paramId, int paramDiameterId, string diameter);
    }
}
