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
        ProductNewDTO AddInstanceProduct(ProductNewDTO productNewDTO, string currentUserName);
        Task<updateProductInstancesRequest> UpdateProduct(updateProductInstancesRequest productDTO, string currentUserName);
        List<ProductParametrDTO> GetInstancesOfProductParameterUpdated(getInstancesOfProductParameterRequest requestParameters);
        addNewProductTypeRequest AddOtherProducts(addNewProductTypeRequest productNewDTO, string currentUserName);
        Task<ProductHistory> DeleteProductInstance(int? id, string currentUserName); 
        byte[] GeneratePDF();
    }
}
