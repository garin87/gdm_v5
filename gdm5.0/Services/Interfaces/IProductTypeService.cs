using gdm5._0.Domain.Models.Filters;
using gdm5._0.DTO;
using gdm5._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
    public interface IProductTypeService : IBaseServices<ProductType>
    {
        public List<ProductParametrDTO> getProductTypeParameters(string nameType);
        public PagedResponseDTO<List<ProductDTO>> getProductTypeInstances(string nameProductType,
            PaginationFilterDTO pageFilter, string route, SortOptionsDTO sortOption, ProductFilter filter);
    }
}
