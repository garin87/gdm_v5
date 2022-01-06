using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.DTO;
using gdm5._0.Models;

namespace gdm5._0.Services.Interfaces
{
    public interface IProductService : IBaseServices<Product>
    {
        Task<Product> UpdateProduct(int id, Product product);
        Task<IEnumerable<ProductDTO>> GetProducts(int id);
        Task<ProductDTO> AddProducts(ProductDTO productDTO);
        Task<ProductDTO> UpdateProducts(ProductDTO productDTO, int id);
        Task<Product> DeleteProducts(int id);
        Task<IEnumerable<ProductDTO>> SortProducs(int id);
        Task<IQueryable<ProductDTO>> SortProducsByParameters(int TypeId, bool StateOrder = true);
        Task<IEnumerable<ProductDTO>> GetProductParam(int id);
        Task<IEnumerable<ProductOrderDTO>> GetParamForOrder(int id);
    }
}
