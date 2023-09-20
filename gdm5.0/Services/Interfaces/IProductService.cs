using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Requests.Product;

namespace gdm5._0.Services.Interfaces
{
    public interface IProductService : IBaseServices<Product>
    {
        string[] GetProductManufacturers();
        string[] GetProductManufacturers(string productName);
        string[] GetProductSuppliers();
        string[] GetProductSuppliers(string productName);
        Task<IEnumerable<ProductDTO>> GetProducts(int id);
        Task<ProductDTO> AddProducts(ProductDTO productDTO);
        ProductNewDTO AddInstanceProduct(ProductNewDTO productNewDTO);
        Task<updateProductInstancesRequest> UpdateProduct(updateProductInstancesRequest productDTO);
        Task<Product> DeleteProducts(int id);
        List<ProductParametrDTO> getInstancesOfProductParameter(getInstancesOfProductParameterRequest requestParameters);
        List<ProductParametrDTO> GetInstancesOfProductParameterUpdated(getInstancesOfProductParameterRequest requestParameters);

        
        addNewProductTypeRequest AddOtherProducts(addNewProductTypeRequest productNewDTO);
        Task<ProductHistory> DeleteProductInstance(Nullable<int> id); 
        byte[] GeneratePDF();
    }
}
