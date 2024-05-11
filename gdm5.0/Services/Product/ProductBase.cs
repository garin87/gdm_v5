using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.Requests.Product;

namespace gdm5._0.Services.ProductS
{
    public class ProductBase<T> : BaseService<T> where T : BaseObject
    {
        public ProductBase(DataContext context) : base(context)
        {
        }
        protected async Task<ProductType> GetProductTypeByNameAsync(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ApplicationException("Product type name cannot be null or empty");
            }

            var productType = await _context.ProductTypes
                .FirstOrDefaultAsync(type => type.NameType.Trim().Equals(typeName.Trim(), StringComparison.OrdinalIgnoreCase));

            if (productType == null)
            {
                throw new ApplicationException($"Product type '{typeName}' does not exist");
            }

            return productType;
        }
        protected int GetOrCreateWarehouse(string warehouseName)
        {
            var warehouse = _context.WareHouse.FirstOrDefault(w => w.Name == warehouseName);
            var idWarehouse = 0;

            if (warehouse == null)
            {
                var newWarehouse = new WareHouse()
                {
                    Id = idWarehouse,
                    Name = warehouseName
                };

                _context.WareHouse.Add(newWarehouse);
                _context.SaveChanges();

                idWarehouse = newWarehouse.Id;
            }
            else
            {
                idWarehouse = warehouse.Id;
            }

            return idWarehouse;
        }
        protected int GetOrCreateCurrency(string currencyName)
        {
            var currency = _context.Currencies.AsNoTracking().FirstOrDefault(c => c.CurrencyName == currencyName);
            var currencyId = 0;

            if (currency == null)
            {
                var newCurrency = new Currency()
                {
                    Id = currencyId,
                    CurrencyName = currencyName
                };

                _context.Currencies.Add(newCurrency);
                _context.SaveChanges();

                currencyId = newCurrency.Id;
            }
            else
            {
                currencyId = currency.Id;
            }

            return currencyId;
        }
        protected int GetOrCreateProductType(string productTypeName)
        {
            if (string.IsNullOrEmpty(productTypeName))
                throw new ApplicationException("Enter valid name product");

            var productType = _context.ProductTypes.FirstOrDefault(pt => pt.NameType == productTypeName.ToLower());
            var idTypeProduct = 0;

            if (productType == null)
            {
                var newTypeProduct = new ProductType()
                {
                    Id = 0,
                    NameType = productTypeName
                };

                _context.ProductTypes.Add(newTypeProduct);
                _context.SaveChanges();
                idTypeProduct = newTypeProduct.Id;
            }
            else
            {
                throw new ApplicationException("Product " + productTypeName + " exists");
            }

            return idTypeProduct;
        }
        protected int GetOrCreateProductTypeHistory(string productTypeName, int deletedProductTypeId)
        {
            var productType = _context.ProductTypeHistory.FirstOrDefault(pt => pt.NameType == productTypeName);
            var idTypeProduct = 0;

            if (productType == null)
            {
                var newTypeProduct = new ProductTypeHistory()
                {
                    Id = 0,
                    NameType = productTypeName,
                    DeletedProductTypeId = 0
                };

                _context.ProductTypeHistory.Add(newTypeProduct);
                _context.SaveChanges();
                idTypeProduct = newTypeProduct.Id;
            }
            else
                idTypeProduct = productType.Id;


            return idTypeProduct;
        }
        protected int GetOrCreateParameterHistory(int? parameterId, int productTypeHistoryId)
        {
            var parameterH = _context.ParameterHistory.FirstOrDefault(pt => pt.DeletedParameterId == parameterId);
            var parameter = _context.Parameters.FirstOrDefault(pt => pt.Id == parameterId);
            var idParameter = 0;

            if (parameterH == null)
            {
                var newParameter = new ParameterHistory()
                {
                    Id = 0,
                    Name = parameter.Name,
                    ProductTypeHistoryId = productTypeHistoryId,
                    DeletedParameterId = parameter.Id,
                    Priority = parameter.Priority,
                    isRequired = parameter.isRequired
                };

                _context.ParameterHistory.Add(newParameter);
                _context.SaveChanges();
                idParameter = newParameter.Id;
            }
            else
                idParameter = parameterH.Id;

            return idParameter;
        }

        protected void ValidateAddOtherProducts(addNewProductTypeRequest product)
        {
            if (string.IsNullOrEmpty(product.Name?.Value))
                throw new ApplicationException("Enter valid name product");

            if (string.IsNullOrEmpty(product.WareHouseName?.Value))
                throw new ApplicationException("Enter valid name warehouse");
        }
    }
}
