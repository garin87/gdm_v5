using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.DTO;
using gdm5._0.Services.Interfaces;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace gdm5._0.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly DataContext _context;
    
        public ProductService(DataContext context) : base(context)
        {
            _context = context;
            
        }

     
    public async Task<Product> UpdateProduct(int id, Product product)
        {
            Product p = await GetItem(product.Id);
            p.Name = product.Name;
            p.ProductNumber = product.ProductNumber;
            p.Quantity = product.Quantity;
            p.StandartCost = product.StandartCost;
            p.ProductTypeId = product.ProductTypeId;
            p.Description = product.Description;
            p.ProductParameters = product.ProductParameters;
            await _context.SaveChangesAsync();
            return  product;
        }

    public async Task<IEnumerable<ProductDTO>> GetProducts(int id)
        {
            var typeParams = await _context.Parameters.Where(t => t.ProductTypeId == id)
                                           .ToListAsync();

            var products = await _context.Products.Include(t => t.ProductParameters)
                                         .Include(h => h.ProductType)   
                                         .Where(t => t.ProductTypeId == id)
                                         .ToListAsync();

            var items = new List<ProductDTO>();

            foreach (var product in products)
            {
                var productDTO = new ProductDTO()
                {
                    ProductId = product.Id,
                    ProductNumber = product.ProductNumber,
                    Manufacturer =product.Manufacturer,
                    Quantity = product.Quantity,
                    ProductStandartCost = product.StandartCost,
                    ProductTypeId = product.ProductTypeId,
                    NameType = product.ProductType.NameType,
                    Description = product.Description
                };

                foreach (var typeParam in typeParams)
                {
                    var paramDTO = new ParameterDTO();                        
                    var value = product.ProductParameters.FirstOrDefault(t => t.ParameterId == typeParam.Id);
                    if (value != null)
                    {
                        paramDTO.Id = value.Id;
                        paramDTO.Value = value.Value;
                    }
                    paramDTO.ParameterId = typeParam.Id;
                    paramDTO.Name = typeParam.Name;
                    

                    productDTO.Parameters.Add(paramDTO);
                }

                 productDTO.Parameters.Where(y => y.ParameterId == 4)
                 .OrderBy(t => t.Value)
                 .ToList();

                 items.Add(productDTO);
            }
         

            return items;

           
        }

    public async Task<ProductDTO> AddProducts(ProductDTO productDTO)
        {
            var EntryProduct = _context.Products.Find(productDTO.ProductId);
            if (EntryProduct != null) return productDTO;

            var product = new Product
            {
                ProductTypeId = productDTO.ProductTypeId,
                ProductNumber = productDTO.ProductNumber,
                //Quantity = productDTO?.Quantity,
                //StandartCost = productDTO?.ProductStandartCost,
                Manufacturer = productDTO.Manufacturer,
                Description = productDTO.Description
            };

            _context.Products.Add(product);

            foreach (var paramDTO in productDTO.Parameters)
            {
                if (string.IsNullOrEmpty(paramDTO.Value))
                    continue;

                var param = new ProductParameter()
                {   ProductId = product.Id,
                    ParameterId = paramDTO.ParameterId,
                    Value = paramDTO.Value
                };
                _context.ProductParameters.Add(param);
            }

            await _context.SaveChangesAsync();

            return productDTO;

    }
        // Add Other Products
    public ProductNewDTO AddOtherProducts(ProductNewDTO productNewDTO)
        {

            if (string.IsNullOrEmpty(productNewDTO.Name?.Value))
                throw new ApplicationException("Enter valid name product");
 
            if (string.IsNullOrEmpty(productNewDTO.WareHouseName?.Value))
                throw new ApplicationException("Enter valid name warehouse");

            var warehouse = _context.WareHouse.FirstOrDefault(warehouse => (warehouse.Name == productNewDTO.WareHouseName.Value));
            var idWarehouse = 0;
            if (warehouse == null)
            {
                var warehouseName = productNewDTO.WareHouseName.Value;
                var newWarehouse = new WareHouse()
                {
                    Id = idWarehouse,
                    Name = warehouseName
                };

                _context.WareHouse.Add(newWarehouse);
                _context.SaveChanges();

                idWarehouse = newWarehouse.Id;
            } else idWarehouse = warehouse.Id;


            var currency = _context.Currencies.FirstOrDefault(currency => 
                                   currency.CurrencyName == productNewDTO.CurrencyName.Value);
            var currencyId = 0;

            if (currency == null)
            {
                var currencyName = productNewDTO.CurrencyName.Value;
                var newCurrency = new Currency()
                {
                    Id = currencyId,
                    CurrencyName = currencyName
                };

                _context.Currencies.Add(newCurrency);
                _context.SaveChanges();

                currencyId = newCurrency.Id;
            }
            else currencyId = currency.Id;

            var productTypeName = _context.ProductTypes.FirstOrDefault(productType => 
                             productType.NameType == productNewDTO.Name.Value);

            var idTypeProduct = 0;
            if (productTypeName == null)
            {
                var pNameType = productNewDTO.Name.Value;

                var newTypeProduct = new ProductType()
                {
                    Id = 0,
                    NameType = pNameType
                };

                _context.ProductTypes.Add(newTypeProduct);
                _context.SaveChanges();
                idTypeProduct = newTypeProduct.Id;
            }
            else throw new ApplicationException("Product " + productNewDTO.Name.Value + " exists");

            var idInstanceProduct = 0;

            double Quantity;
            double StandartCost;
            double PrimeCost;
            DateTime DateOfReceipt;

            CallParseDouble(productNewDTO.Quantity?.Value, out Quantity);
            CallParseDouble(productNewDTO.StandartCost?.Value, out StandartCost);
            CallParseDouble(productNewDTO.PrimeCost?.Value, out PrimeCost);
            if (!String.IsNullOrEmpty(productNewDTO.DateOfReceipt?.Value)) DateTime.TryParse(productNewDTO.DateOfReceipt?.Value, out DateOfReceipt);
            else DateOfReceipt = new DateTime();

            var instanceProduct = new Product
            {
                Id = idInstanceProduct,
                ProductTypeId = idTypeProduct,
                Name = productNewDTO.Name?.Value,
                ProductNumber = productNewDTO.ProductNumber?.Value,
                Quantity = Quantity,
                StandartCost = StandartCost,
                Manufacturer = productNewDTO.Manufacturer?.Value,
                Description = productNewDTO.Description?.Value,
                DateOfReceipt = DateOfReceipt,
                WareHouseId = idWarehouse,
                PrimeCost = PrimeCost,
                CurrencyId = currencyId
            };

            _context.Products.Add(instanceProduct);
            _context.SaveChanges();
            idInstanceProduct = instanceProduct.Id;

            foreach (var parameter in productNewDTO.Parameters)
            {
                if (string.IsNullOrEmpty(parameter.Name)) continue;

                var existingParam = _context.Parameters?
                    .Where(item => item.Name == parameter.Name && item.ProductTypeId == idTypeProduct)
                    .FirstOrDefault();

                var idParam = 0;
                if (existingParam != null)
                {
                    idParam = existingParam.Id;
                }
                else
                {
                    var inctanceParameter = new Parameter()
                    {
                        ProductTypeId = idTypeProduct,
                        Name = parameter.Name,
                    };

                    _context.Parameters.Add(inctanceParameter);
                    _context.SaveChanges();

                    idParam = inctanceParameter.Id;
                }


                var param = new ProductParameter()
                {
                    ProductId = idInstanceProduct,
                    ParameterId = idParam,
                    Value = parameter.Value
                };

                _context.ProductParameters.Add(param);
            }
            _context.SaveChanges();

            return productNewDTO;
        }

    public ProductNewDTO AddInstanceProduct(ProductNewDTO productNewDTO)
    {

            if (string.IsNullOrEmpty(productNewDTO.Name?.Value))
                throw new ApplicationException("Enter valid name product");

            if (string.IsNullOrEmpty(productNewDTO.WareHouseName?.Value))
                throw new ApplicationException("Enter valid name warehouse");

            var warehouse = _context.WareHouse.FirstOrDefault(warehouse => (warehouse.Name == productNewDTO.WareHouseName.Value));
            var idWarehouse = 0;
            if (warehouse != null)
            {
                idWarehouse = warehouse.Id;
            }
            else throw new ApplicationException("Entered name of warehouse does not exist"); ;


            var currency = _context.Currencies.FirstOrDefault(currency =>
                                   currency.CurrencyName == productNewDTO.CurrencyName.Value);
            var currencyId = 0;

            if (currency != null)
            {
                currencyId = currency.Id;
            }
            else throw new ApplicationException("Entered name of currency does not exist");

            var productTypeName = _context.ProductTypes.FirstOrDefault(productType => productType.NameType == productNewDTO.Name.Value);
            var idTypeProduct = 0;
            if (productTypeName != null)
            {
                idTypeProduct = productTypeName.Id;
            }
            else throw new ApplicationException("Entered name of product does not exist");

            double Quantity;
            double StandartCost;
            double PrimeCost;
            DateTime DateOfReceipt;

            CallParseDouble(productNewDTO.Quantity?.Value, out Quantity);
            CallParseDouble(productNewDTO.StandartCost?.Value, out StandartCost);
            CallParseDouble(productNewDTO.PrimeCost?.Value, out PrimeCost);
            if (!String.IsNullOrEmpty(productNewDTO.DateOfReceipt?.Value)) DateTime.TryParse(productNewDTO.DateOfReceipt?.Value, out DateOfReceipt);
            else DateOfReceipt = new DateTime();
            var idInstanceProduct = 0;
            var instanceProduct = new Product
            {
                Id = idInstanceProduct,
                ProductTypeId = idTypeProduct,
                Name = productNewDTO.Name?.Value,
                ProductNumber = productNewDTO.ProductNumber?.Value,
                Quantity = Quantity,
                StandartCost = StandartCost,
                Manufacturer = productNewDTO.Manufacturer?.Value,
                Description = productNewDTO.Description?.Value,
                DateOfReceipt = DateOfReceipt,
                WareHouseId = idWarehouse,
                PrimeCost = PrimeCost,
                CurrencyId = currencyId
            };

            _context.Products.Add(instanceProduct);
            _context.SaveChanges();
            idInstanceProduct = instanceProduct.Id;

           

            foreach (var parameter in productNewDTO.Parameters)
            {
                if (string.IsNullOrEmpty(parameter.Name)) continue;

                var existingParam = _context.Parameters?
                    .Where(item => item.Name == parameter.Name && item.ProductTypeId == idTypeProduct)
                    .FirstOrDefault();

                var idParam = 0;
                if (existingParam != null)
                {
                    idParam = existingParam.Id;
                }
                else continue;
                


                var param = new ProductParameter()
                {
                    ProductId = idInstanceProduct,
                    ParameterId = idParam,
                    Value = parameter.Value
                };

                _context.ProductParameters.Add(param);
               // _context.SaveChanges();
            }
            _context.SaveChanges();

            return productNewDTO;
        }

    public async Task<ProductDTO> UpdateProducts(ProductDTO productDTO, int id)
        {
            var product = _context.Products.Include(t => t.ProductParameters)
                                           .Where(t => t.Id == id)
                                           .FirstOrDefault();

            //product.StandartCost = productDTO.ProductStandartCost;
            product.ProductTypeId = productDTO.ProductTypeId;
            product.ProductNumber = productDTO.ProductNumber;
            //  product.Quantity = productDTO.Quantity;
            // product.StandartCost = productDTO.ProductStandartCost;
            product.Manufacturer = productDTO.Manufacturer;
            product.Description = productDTO.Description;

            foreach (var paramDTO in productDTO.Parameters)
            {
                if (string.IsNullOrEmpty(paramDTO.Value))
                    continue;

                if (paramDTO.Id == 0)
                {
                    var param = new ProductParameter()
                    {
                        ProductId = product.Id,
                        ParameterId = paramDTO.ParameterId,
                        Value = paramDTO.Value
                    };
                    _context.ProductParameters.Add(param);
                }
                else
                {
                    var existingParam = product.ProductParameters.Where(p => p.Id == paramDTO.Id).FirstOrDefault();
                    if (existingParam != null)
                    {
                        existingParam.Value = paramDTO.Value;
                    }

                }

            }

            await _context.SaveChangesAsync();


            return productDTO;

     }

    public async Task<Product> DeleteProducts(int id)
    {
            var product = _context.Products
                   .Include(p => p.ProductType)
                   .Include(pp => pp.ProductParameters)
                   .Where(i => i.Id == id).FirstOrDefault();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

         return product;
    }

    public async Task<IEnumerable<ProductDTO>> SortProducs(int id)
        {
            var products = await _context.Products
              .Include(p => p.ProductParameters)
               
              .Where(prod => prod.ProductTypeId == id)
              .OrderBy(v => v.ProductParameters.Where(g => g.ParameterId == 4)
                   .OrderBy(t => Convert.ToInt32(t.Value)).FirstOrDefault().Value)
              .Select(prod => new ProductDTO
              {
                  ProductId = prod.Id,
                  ProductNumber = prod.ProductNumber,
                  NameType = prod.ProductType.NameType,
                  Quantity = prod.Quantity,
                  ProductStandartCost = prod.StandartCost,
                  ProductTypeId = prod.ProductTypeId,
                  Manufacturer = prod.Manufacturer,
                  Description = prod.Description,
                  Parameters = prod.ProductParameters
                    .Select(par => new ParameterDTO
                    {
                        Id = par.Id,
                        ParameterId = par.ParameterId,
                        Value = par.Value,
                        Name = par.Parameter.Name
                    })
                    .ToList()
              })
              .ToListAsync();


            return products;
        }

    public async Task<IQueryable<ProductDTO>> SortProducsByParameters(int TypeId, bool StateOrder = true)
        {
            string param = "Стандарт";
            int paramId = 2; // тип штока 
            int paramDiameterId = 4;
            string diameter = "60";

            var products = await _context.ProductParameters
                           .Include(p => p.Product)
                           .Where(p => p.Product.ProductTypeId == TypeId && p.Value == param && p.ParameterId == paramId)
                           .Select(p => p.Product)
                           .Include(p => p.ProductType)
                           .Include(p => p.ProductParameters)
                           .ToListAsync();


            if (StateOrder == true)
            {
                products = products.OrderBy(v => v.ProductParameters.Where(g => g.ParameterId == paramDiameterId)
                   .OrderBy(t => Convert.ToInt32(t.Product.Quantity)).FirstOrDefault().Product.Quantity).ToList();
                
            }
            else
            {
                products = products.OrderByDescending(v => v.ProductParameters.Where(g => g.ParameterId == paramDiameterId)
                .OrderByDescending(t => t.Product.Quantity).FirstOrDefault().Product.Quantity).ToList();
            }

            var items = new List<ProductDTO>();

            foreach (var product in products)
            {
                var foundParam = product.ProductParameters.Where(pp => pp.ParameterId == paramDiameterId && pp.Value == diameter)
                    .FirstOrDefault();
                if (foundParam == null)
                    continue;

                var productDTO = new ProductDTO()
                {
                    ProductId = product.Id,
                    ProductNumber = product.ProductNumber,
                    Manufacturer = product.Manufacturer,
                    Quantity = product.Quantity,
                    ProductStandartCost = product.StandartCost,
                    ProductTypeId = product.ProductTypeId,
                    Description = product.Description, 
                    NameType = product.ProductType.NameType
                };

                foreach (var typeParam in product.ProductParameters)
                {
                    var paramDTO = new ParameterDTO();
                    var value = product.ProductParameters.FirstOrDefault(t => t.ParameterId == typeParam.Id);
                    if (value != null)
                    {
                        paramDTO.Id = value.Id;
                        paramDTO.Value = value.Value;
                    }
                    paramDTO.Id = typeParam.Id;
                    paramDTO.Name = typeParam.Product.ProductType.NameType;
                    paramDTO.ParameterId = typeParam.ParameterId;
                    paramDTO.Value = typeParam.Value;

                    productDTO.Parameters.Add(paramDTO);
                }

                items.Add(productDTO);
            }

            return items.AsQueryable();
        }

    public async Task<IEnumerable<ProductDTO>> GetProductParam(int id)
        {
            var products = await _context.Products
              .Include(p => p.ProductParameters)
              .Where(prod => prod.Id == id)
              .Select(prod => new ProductDTO
              {
                  ProductId = prod.Id,
                  ProductNumber = prod.ProductNumber,
                  Quantity = prod.Quantity,
                  ProductStandartCost = prod.StandartCost,
                  ProductTypeId = prod.ProductTypeId,
                  Manufacturer = prod.Manufacturer,
                  Description = prod.Description,
                  Parameters = prod.ProductParameters
                    .Select(par => new ParameterDTO
                    {
                        Id = par.Id,
                        ParameterId = par.ParameterId,
                        Value = par.Value,
                        Name = par.Parameter.Name
                    })

                    .ToList()
              })

              .ToListAsync();


            return products;
        }

    public async Task<IEnumerable<ProductOrderDTO>> GetParamForOrder(int id)
        {
            var param = await _context.Products
                                      .Where(p => p.Id == id)
                                      .Select(m => new ProductOrderDTO
                                      {
                                          ProductNumber = m.ProductNumber,
                                          ProductStandartCost = m.StandartCost
                                      }).ToListAsync();

            return param;

        }

        public static void CallParseDouble(string valueText, out Double dd)
        {
            if (!String.IsNullOrEmpty(valueText))
            {
                double d;
                bool result = double.TryParse(valueText, out d);

                if (result)
                    dd = d;
                else
                    dd = 0;
            }
            else dd = 0;
        }
    }
}



//var typeP = _context.ProductTypes?.Where(name => name.NameType == productDTO.NameType).FirstOrDefault();

//if (typeP != null)
//{
//    idType = typeP.Id;
//}
//else
//{
//    nameType = productDTO.NameType;
//    var typeProduct = new ProductType()
//    {
//        Id = 0,
//        NameType = nameType,
//    };

//    _context.ProductTypes.Add(typeProduct);
//    await _context.SaveChangesAsync();

//    idType = typeProduct.Id;
//}

//typeP = _context.ProductTypes?.Where(name => name.NameType == productDTO.NameType).FirstOrDefault();
//var product = new Product
//{
//    ProductTypeId = idType,
//    Name = productDTO.Name,
//    ProductNumber = productDTO.ProductNumber,
//    Quantity = productDTO.Quantity,
//    StandartCost = productDTO.ProductStandartCost,
//    Manufacturer = productDTO.Manufacturer,
//    Description = productDTO.Description,
//};


//_context.Products.Add(product);
//await _context.SaveChangesAsync();

//foreach (var paramDTO in productDTO.Parameters)
//{
//    if (string.IsNullOrEmpty(paramDTO.Value))
//        continue;

//    var existParam = _context.Parameters?
//        .Where(item => item.Name == paramDTO.Name && item.ProductTypeId == idType)
//        .FirstOrDefault();

//    var idParam = 0;
//    if(existParam != null)
//    {
//        idParam = existParam.Id;
//    }
//    else
//    {
//        var parameters = new Parameter()
//        {
//            ProductTypeId = typeP.Id,
//            Name = paramDTO.Name,
//        };

//        _context.Parameters.Add(parameters);
//        idParam = parameters.Id;
//    }


//    var param = new ProductParameter()
//    {
//        ProductId = product.Id,
//        ParameterId = idParam,
//        Value = paramDTO.Value
//    };

//    _context.ProductParameters.Add(param);
//}

//await _context.SaveChangesAsync();