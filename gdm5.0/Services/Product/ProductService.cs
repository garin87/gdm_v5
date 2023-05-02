using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.DTO;
using gdm5._0.Services.Interfaces;
using gdm5._0.Requests.Product;
using Microsoft.AspNetCore.Http;

namespace gdm5._0.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(DataContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public string[] GetProductManufacturers()
        {
            return _context.Products.Select(product => product.Manufacturer).Distinct().ToArray();
        }


        public string[] GetProductManufacturers(string productName)
        {
            if (string.IsNullOrEmpty(productName))
                throw new ApplicationException("Enter valid name product");


            return _context.Products.Include(product=> product.ProductType)
                                    .Where(product=> product.ProductType.NameType == productName)
                                    .Select(product => product.Manufacturer)
                                    .Distinct().ToArray();
        }

       
        public string[] GetProductSuppliers()
        {
            return _context.Products.Select(customer => customer.Supplier).Distinct().ToArray();
        }

        public string[] GetProductSuppliers(string productName)
        {
            if (string.IsNullOrEmpty(productName))
                throw new ApplicationException("Enter valid name product");

            return _context.Products.Include(product => product.ProductType)
                                    .Where(product => product.ProductType.NameType == productName)
                                    .Select(customer => customer.Supplier).Distinct().ToArray();
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
                    Manufacturer = product.Manufacturer,
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
                { ProductId = product.Id,
                    ParameterId = paramDTO.ParameterId,
                    Value = paramDTO.Value
                };
                _context.ProductParameters.Add(param);
            }

            await _context.SaveChangesAsync();

            return productDTO;

        }

        public addNewProductTypeRequest AddOtherProducts(addNewProductTypeRequest product)
        {
            this.ValidateAddOtherProducts(product);

            var idWarehouse = GetOrCreateWarehouse(product.WareHouseName.Value);
            var currencyId = GetOrCreateCurrency(product.CurrencyName.Value);
            var idTypeProduct = GetOrCreateProductType(product.Name.Value);

            
            var idInstanceProduct = 0;

            double Quantity;
            double StandartCost;
            double PrimeCost;
            DateTime DateOfReceipt;

            CallParseDouble(product.Quantity?.Value, out Quantity);
            CallParseDouble(product.StandartCost?.Value, out StandartCost);
            CallParseDouble(product.PrimeCost?.Value, out PrimeCost);
            if (!String.IsNullOrEmpty(product.DateOfReceipt?.Value)) DateTime.TryParse(product.DateOfReceipt?.Value, out DateOfReceipt);
            else DateOfReceipt = new DateTime();

            var instanceProduct = new Product
            {
                Id = idInstanceProduct,
                ProductTypeId = idTypeProduct,
                Name = product.Name?.Value,
                ProductNumber = product.ProductNumber?.Value,
                Quantity = Quantity,
                StandartCost = StandartCost,
                Manufacturer = product.Manufacturer?.Value,
                Description = product.Description?.Value,
                DateOfReceipt = DateOfReceipt,
                WareHouseId = idWarehouse,
                PrimeCost = PrimeCost,
                CurrencyId = currencyId
            };

            _context.Products.Add(instanceProduct);
            _context.SaveChanges();
            idInstanceProduct = instanceProduct.Id;

            foreach (var parameter in product.Parameters)
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
                        Priority = parameter.NavPriority.Value,
                        NameType = parameter.Type

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

            return product;
        }

        // Add Other Products
        //public addNewProductTypeRequest AddOtherProducts(addNewProductTypeRequest product)
        //{

        //    this.ValidateAddOtherProducts(product);

        //    var warehouse = _context.WareHouse.FirstOrDefault(warehouse => (warehouse.Name == product.WareHouseName.Value));
        //    var idWarehouse = 0;
        //    if (warehouse == null)
        //    {
        //        var warehouseName = product.WareHouseName.Value;
        //        var newWarehouse = new WareHouse()
        //        {
        //            Id = idWarehouse,
        //            Name = warehouseName
        //        };

        //        _context.WareHouse.Add(newWarehouse);
        //        _context.SaveChanges();

        //        idWarehouse = newWarehouse.Id;
        //    }
        //    else idWarehouse = warehouse.Id;


        //    var currency = _context.Currencies.FirstOrDefault(currency =>
        //                           currency.CurrencyName == product.CurrencyName.Value);
        //    var currencyId = 0;

        //    if (currency == null)
        //    {
        //        var currencyName = product.CurrencyName.Value;
        //        var newCurrency = new Currency()
        //        {
        //            Id = currencyId,
        //            CurrencyName = currencyName
        //        };

        //        _context.Currencies.Add(newCurrency);
        //        _context.SaveChanges();

        //        currencyId = newCurrency.Id;
        //    }
        //    else currencyId = currency.Id;

        //    var productTypeName = _context.ProductTypes.FirstOrDefault(productType =>
        //                     productType.NameType == product.Name.Value);

        //    var idTypeProduct = 0;
        //    if (productTypeName == null)
        //    {
        //        var pNameType = product.Name.Value;

        //        var newTypeProduct = new ProductType()
        //        {
        //            Id = 0,
        //            NameType = pNameType
        //        };

        //        _context.ProductTypes.Add(newTypeProduct);
        //        _context.SaveChanges();
        //        idTypeProduct = newTypeProduct.Id;
        //    }
        //    else throw new ApplicationException("Product " + product.Name.Value + " exists");

        //    var idInstanceProduct = 0;

        //    double Quantity;
        //    double StandartCost;
        //    double PrimeCost;
        //    DateTime DateOfReceipt;

        //    CallParseDouble(product.Quantity?.Value, out Quantity);
        //    CallParseDouble(product.StandartCost?.Value, out StandartCost);
        //    CallParseDouble(product.PrimeCost?.Value, out PrimeCost);
        //    if (!String.IsNullOrEmpty(product.DateOfReceipt?.Value)) DateTime.TryParse(product.DateOfReceipt?.Value, out DateOfReceipt);
        //    else DateOfReceipt = new DateTime();

        //    var instanceProduct = new Product
        //    {
        //        Id = idInstanceProduct,
        //        ProductTypeId = idTypeProduct,
        //        Name = product.Name?.Value,
        //        ProductNumber = product.ProductNumber?.Value,
        //        Quantity = Quantity,
        //        StandartCost = StandartCost,
        //        Manufacturer = product.Manufacturer?.Value,
        //        Description = product.Description?.Value,
        //        DateOfReceipt = DateOfReceipt,
        //        WareHouseId = idWarehouse,
        //        PrimeCost = PrimeCost,
        //        CurrencyId = currencyId
        //    };

        //    _context.Products.Add(instanceProduct);
        //    _context.SaveChanges();
        //    idInstanceProduct = instanceProduct.Id;

        //    foreach (var parameter in product.Parameters)
        //    {
        //        if (string.IsNullOrEmpty(parameter.Name)) continue;

        //        var existingParam = _context.Parameters?
        //            .Where(item => item.Name == parameter.Name && item.ProductTypeId == idTypeProduct)
        //            .FirstOrDefault();

        //        var idParam = 0;
        //        if (existingParam != null)
        //        {
        //            idParam = existingParam.Id;
        //        }
        //        else
        //        {
        //            var inctanceParameter = new Parameter()
        //            {
        //                ProductTypeId = idTypeProduct,
        //                Name = parameter.Name,
        //                Priority = parameter.NavPriority.Value,
        //                NameType = parameter.Type

        //            };

        //            _context.Parameters.Add(inctanceParameter);
        //            _context.SaveChanges();

        //            idParam = inctanceParameter.Id;
        //        }

        //        var param = new ProductParameter()
        //        {
        //            ProductId = idInstanceProduct,
        //            ParameterId = idParam,
        //            Value = parameter.Value
        //        };

        //        _context.ProductParameters.Add(param);
        //    }
        //    _context.SaveChanges();

        //    return product;
        //}

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
            double PrimeCostEUR;
            double PrimeCostUSD;
            DateTime DateOfReceipt;

            CallParseDouble(productNewDTO.Quantity?.Value, out Quantity);
            CallParseDouble(productNewDTO.StandartCost?.Value, out StandartCost);
            CallParseDouble(productNewDTO.PrimeCost?.Value, out PrimeCost);
            CallParseDouble(productNewDTO.PrimeCostEUR?.Value, out PrimeCostEUR);
            CallParseDouble(productNewDTO.PrimeCostUSD?.Value, out PrimeCostUSD);
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
                PrimeCostEUR = PrimeCostEUR,
                PrimeCostUSD = PrimeCostUSD,
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

        public List<ProductParametrDTO> getInstancesOfProductParameter(string nameType, string nameParam, bool isParameter)
        {

            var productType = _context.ProductTypes.Where(type => type.NameType == nameType.Trim()).FirstOrDefault();

            if (productType == null)
                throw new ApplicationException("Product name " + nameType + " does not exist");


            SortOptionsDTO parametrOption = new SortOptionsDTO() {
                IsParameter = false

            };
            parametrOption.Name = nameParam.Trim().ToLower();
            parametrOption.IsParameter = isParameter;
            var productParameters = new List<ProductParameter>();
            var parameters = new List<Parameter>();
            List<dynamic> parameterValues = new List<dynamic>();
            if (isParameter)
            {
                //var parametrs = _context.Parameters.Where(param => param.ProductTypeId == productType.Id).ToList();
                var paramId = _context.Parameters.Where(param => param.ProductTypeId == productType.Id)
                                                 .Where(el => el.Name.Trim().ToLower() == parametrOption.Name).FirstOrDefault()?.Id;

                if (paramId != null)
                {
                    // productParameters = _context.ProductParameters.Where(type => type.ParameterId == paramId).ToList();
                    parameterValues = _context.ProductParameters.Where(type => type.ParameterId == paramId && !string.IsNullOrEmpty(type.Value))
                                                                // .Where(el => !string.IsNullOrEmpty(el.Value))
                                                                .Select(field => field.Value as dynamic).Distinct().ToList();
                }
            }
            else
            {
                //var temp = _context.Products.Where(param => param.ProductTypeId == productType.Id).ToList();
                // var fields = _context.Products.Where(param => param.ProductTypeId == productType.Id)
                //.Select( el => el.getSortField(parametrOption, parameters) ).Distinct().ToList();

                parameterValues = _context.Products.Where(param => param.ProductTypeId == productType.Id)
                                          .Select(el => el.getSortField(parametrOption, parameters)).Distinct().ToList();
            }




            List<ProductParametrDTO> PproductParameters = new List<ProductParametrDTO>();

            parameterValues = new HashSet<dynamic>(parameterValues).ToList();
            if (parameterValues != null)
            {
                foreach (var value in parameterValues)
                {
                    if (value != null)
                        PproductParameters.Add(new ProductParametrDTO { Name = nameParam, Value = value });
                };
            }

            return PproductParameters.OrderBy(el => el.Value).ToList<ProductParametrDTO>();
        }

        public async Task<updateProductInstancesRequest> UpdateProduct(updateProductInstancesRequest productDTO)
        {

            if (productDTO.ProductId == 0)
                throw new ApplicationException("Enter valid ProductId");

            var warehouse = _context.WareHouse.FirstOrDefault(warehouse => (warehouse.Name == productDTO.warehouse.Value));
            var idWarehouse = 0;

            if (warehouse != null)
            {
                idWarehouse = warehouse.Id;
            }
            else throw new ApplicationException("Entered name of warehouse does not exist"); ;


            var currency = _context.Currencies.FirstOrDefault(currency => currency.CurrencyName == productDTO.currency.Value);
            var currencyId = 0;

            if (currency != null)
            {
                currencyId = currency.Id;
            }
            else throw new ApplicationException("Entered name of currency does not exist");

            var product = _context.Products.Include(product => product.ProductType)
                                           .Include(product => product.ProductParameters)
                                           .Include(product => product.WareHouse)
                                           .Include(product => product.Currency)
                                           .Where(t => t.Id == productDTO.ProductId)
                                           .FirstOrDefault();

            double Quantity;
            double StandartCost;
            double PrimeCost;
            double PrimeCostUSD;
            double PrimeCostEUR;
            DateTime DateOfReceipt = product.DateOfReceipt;

            CallParseDouble(productDTO.quantity?.Value, out Quantity);
            CallParseDouble(productDTO.standartCost?.Value, out StandartCost);
            CallParseDouble(productDTO.primeCost?.Value, out PrimeCost);
            CallParseDouble(productDTO.PrimeCostUSD?.Value, out PrimeCostUSD);
            CallParseDouble(productDTO.PrimeCostEUR?.Value, out PrimeCostEUR);
            if (!String.IsNullOrEmpty(productDTO.dateOfReceipt?.Value)) DateTime.TryParse(productDTO.dateOfReceipt?.Value, out DateOfReceipt);

            product.ProductNumber = productDTO.productNumber?.Value;
            product.Manufacturer = productDTO.manufacturer?.Value;
            product.Quantity = Quantity;
            product.StandartCost = StandartCost;
            product.PrimeCost = PrimeCost;

            if (PrimeCostUSD != 0 && PrimeCostEUR != 0)
            {
                product.PrimeCostUSD = PrimeCostUSD;
                product.PrimeCostEUR = PrimeCostEUR;
            }
           
            product.Description = productDTO.description?.Value;
            product.DateOfReceipt = DateOfReceipt;
            product.WareHouseId = idWarehouse;
            product.CurrencyId = currencyId;
            product.DateOfLastChanged = DateTime.Now;
            product.LastEditedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name;
            await _context.SaveChangesAsync();
            foreach (var paramDTO in productDTO.parameters)
            {
                if (!string.IsNullOrEmpty(paramDTO.Name))
                {
                    var existingParam = await _context.Parameters.Where(item => item.ProductTypeId == product.ProductTypeId)
                                                           .Where(item => item.Name == paramDTO.Name).FirstOrDefaultAsync();

                    if (existingParam != null)
                    {
                        var productParameters = product.ProductParameters.Where(p => p.ParameterId == existingParam.Id).FirstOrDefault();

                        if (productParameters != null & !string.IsNullOrWhiteSpace(paramDTO?.Value))
                        {
                            productParameters.Value = paramDTO.Value;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();


            return productDTO;
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

        public async Task<ProductHistory> DeleteProductInstance(int? id)
        {
            if (!id.HasValue) throw new ApplicationException("ProductId does not exist");

            var product = _context.Products
                   .Include(p => p.ProductType)
                    .ThenInclude(parameters => parameters.Parameters)
                    .ThenInclude(productParameters => productParameters.ProductParameters)
                   .Where(i => i.Id == id).FirstOrDefault();


            if (product == null) throw new ApplicationException("ProductId does not exist");

            var productTypeHistoryId = GetOrCreateProductTypeHistory(product.ProductType.NameType, product.ProductType.Id);
            var deletedProduct = new ProductHistory()
            {
                Name = product.Name,
                ProductNumber = product.ProductNumber,
                Quantity = product.Quantity,
                PrimeCost = product.PrimeCost,
                PrimeCostEUR = product.PrimeCostEUR,
                PrimeCostUSD = product.PrimeCostUSD,
                StandartCost = product.StandartCost,
                Supplier = product.Supplier,
                ProductStandartCost = product.StandartCost,
                Manufacturer = product.Manufacturer,
                Description = product.Description,
                ProductDeleted = true,
                DateOfChange = DateTime.Now,
                DateOfReceipt = product.DateOfReceipt,
                ProductTypeHistoryId = productTypeHistoryId,
                DeletedProductTypeId = product.ProductTypeId,
                CurrencyId = product.CurrencyId,
                WareHouseId = product.WareHouseId,
                DeletedProductId = product.Id,
                UserName = _httpContextAccessor.HttpContext.User.Identity.Name
            };

            _context.ProductHistory.Add(deletedProduct);

            await _context.SaveChangesAsync();

            foreach (var productValue in product.ProductParameters)
            {
                var idPameterHistory = this.GetOrCreateParameterHistory(productValue.ParameterId, productTypeHistoryId);
                var deletedProductParameters = new ProductParameterHistory
                {
                    ProductHistoryId = deletedProduct.Id,
                    ParameterHistoryId = idPameterHistory,
                    Value = productValue.Value
                };
                _context.ProductParameterHistory.Add(deletedProductParameters);
            }
            await _context.SaveChangesAsync();

            _context.Products.Remove(product);


            await this.AddOrderProductHistoryAsync(id, deletedProduct.Id);

            await _context.SaveChangesAsync();


            return deletedProduct;
        }

      

        public async Task<ProductHistory> DeleteProductInstance2(int? id)
        {
            if (!id.HasValue) throw new ApplicationException("ProductId does not exist");

            var product = await _context.Products
                .Include(p => p.ProductType.Parameters)
                .Include(p => p.ProductParameters)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) throw new ApplicationException("ProductId does not exist");

            var deletedProduct = new ProductHistory()
            {
                Name = product.Name,
                ProductNumber = product.ProductNumber,
                Quantity = product.Quantity,
                PrimeCost = product.PrimeCost,
                ProductStandartCost = product.StandartCost,
                Manufacturer = product.Manufacturer,
                Description = product.Description,
                ProductDeleted = true,
                DateOfChange = DateTime.Now,
                ProductTypeHistoryId = product.ProductTypeId,
                CurrencyId = product.CurrencyId,
                WareHouseId = product.WareHouseId,
                DeletedProductId = product.Id,
                UserName = _httpContextAccessor.HttpContext.User.Identity.Name
            };

            var productParameterDTO = product.ProductType.Parameters
                .Select(parameter => new ProductParametrDTO
                {
                    Id = parameter.Id,
                    Name = parameter.Name,
                    NameType = parameter.NameType,
                    Priority = parameter.Priority
                })
                .ToList();

            var parameterHistories = product.ProductType.Parameters
                .Select(parameter => new ParameterHistory
                {
                    Name = parameter.Name,
                    NameType = parameter.NameType,
                    Priority = parameter.Priority
                })
                .ToList();

            var deletedProductParameters = product.ProductParameters
                .Select(productValue => new ProductParameterHistory
                {
                    ProductHistoryId = deletedProduct.Id,
                    ParameterHistoryId = parameterHistories
                        .FirstOrDefault(param => param.Id == productValue.ParameterId)?.Id ?? default,
                    Value = productValue.Value
                })
                .ToList();

            _context.ProductHistory.Add(deletedProduct);
            _context.ParameterHistory.AddRange(parameterHistories);
            _context.ProductParameterHistory.AddRange(deletedProductParameters);
            _context.Products.Remove(product);

            await this.AddOrderProductHistoryAsync(id, deletedProduct.Id);
            await _context.SaveChangesAsync();

            return deletedProduct;
        }
        private async Task AddOrderProductHistoryAsync(int? productId, int? deletedProductId)
        {
            if (productId == null)
            {
                throw new ArgumentException("ProductId does not exist.");
            }

            if (deletedProductId == null)
            {
                throw new ArgumentException("DeletedProductId does not exist.");
            }

            var orderProducts = await _context.OrderProducts
                                              .Where(op => op.ProductId == productId)
                                              .ToListAsync();

            if (orderProducts.Any())
            {
                var orderProductHistories = orderProducts.Select(op => new OrderProductHistory
                {
                    Quantity = op.Quantity,
                    ProductHistoryId = deletedProductId.Value,
                    OrderId = op.OrderId,
                    TotalPrice = op.TotalPrice,
                    TaxNDS = op.TaxNDS,
                    Markup = op.Markup
                }).ToList();

                _context.OrderProductHistory.AddRange(orderProductHistories);
                await _context.SaveChangesAsync();
            }
        }

        //public async Task<IEnumerable<ProductDTO>> SortProducs(int id)
        //{
        //    var products = await _context.Products
        //      .Include(p => p.ProductParameters)

        //      .Where(prod => prod.ProductTypeId == id)
        //      .OrderBy(v => v.ProductParameters.Where(g => g.ParameterId == 4)
        //           .OrderBy(t => Convert.ToInt32(t.Value)).FirstOrDefault().Value)
        //      .Select(prod => new ProductDTO
        //      {
        //          ProductId = prod.Id,
        //          ProductNumber = prod.ProductNumber,
        //          NameType = prod.ProductType.NameType,
        //          Quantity = prod.Quantity,
        //          ProductStandartCost = prod.StandartCost,
        //          ProductTypeId = prod.ProductTypeId,
        //          Manufacturer = prod.Manufacturer,
        //          Description = prod.Description,
        //          Parameters = prod.ProductParameters
        //            .Select(par => new ParameterDTO
        //            {
        //                Id = par.Id,
        //                ParameterId = par.ParameterId,
        //                Value = par.Value,
        //                Name = par.Parameter.Name
        //            })
        //            .ToList()
        //      })
        //      .ToListAsync();


        //    return products;
        //}

        //public async Task<IQueryable<ProductDTO>> SortProducsByParameters(int TypeId, bool StateOrder = true)
        //{
        //    string param = "Стандарт";
        //    int paramId = 2; // тип штока 
        //    int paramDiameterId = 4;
        //    string diameter = "60";

        //    var products = await _context.ProductParameters
        //                   .Include(p => p.Product)
        //                   .Where(p => p.Product.ProductTypeId == TypeId && p.Value == param && p.ParameterId == paramId)
        //                   .Select(p => p.Product)
        //                   .Include(p => p.ProductType)
        //                   .Include(p => p.ProductParameters)
        //                   .ToListAsync();


        //    if (StateOrder == true)
        //    {
        //        products = products.OrderBy(v => v.ProductParameters.Where(g => g.ParameterId == paramDiameterId)
        //                           .OrderBy(t => Convert.ToInt32(t.Product.Quantity)).FirstOrDefault().Product.Quantity).ToList();

        //    }
        //    else
        //    {
        //        products = products.OrderByDescending(v => v.ProductParameters.Where(g => g.ParameterId == paramDiameterId)
        //                           .OrderByDescending(t => t.Product.Quantity).FirstOrDefault().Product.Quantity).ToList();
        //    }

        //    var items = new List<ProductDTO>();

        //    foreach (var product in products)
        //    {
        //        var foundParam = product.ProductParameters.Where(pp => pp.ParameterId == paramDiameterId && pp.Value == diameter)
        //            .FirstOrDefault();
        //        if (foundParam == null)
        //            continue;

        //        var productDTO = new ProductDTO()
        //        {
        //            ProductId = product.Id,
        //            ProductNumber = product.ProductNumber,
        //            Manufacturer = product.Manufacturer,
        //            Quantity = product.Quantity,
        //            ProductStandartCost = product.StandartCost,
        //            ProductTypeId = product.ProductTypeId,
        //            Description = product.Description, 
        //            NameType = product.ProductType.NameType
        //        };

        //        foreach (var typeParam in product.ProductParameters)
        //        {
        //            var paramDTO = new ParameterDTO();
        //            var value = product.ProductParameters.FirstOrDefault(t => t.ParameterId == typeParam.Id);
        //            if (value != null)
        //            {
        //                paramDTO.Id = value.Id;
        //                paramDTO.Value = value.Value;
        //            }
        //            paramDTO.Id = typeParam.Id;
        //            paramDTO.Name = typeParam.Product.ProductType.NameType;
        //            paramDTO.ParameterId = typeParam.ParameterId;
        //            paramDTO.Value = typeParam.Value;

        //            productDTO.Parameters.Add(paramDTO);
        //        }

        //        items.Add(productDTO);
        //    }

        //    return items.AsQueryable();
        //}

        //public async Task<IEnumerable<ProductDTO>> GetProductParam(int id)
        //{
        //    var products = await _context.Products
        //      .Include(p => p.ProductParameters)
        //      .Where(prod => prod.Id == id)
        //      .Select(prod => new ProductDTO
        //      {
        //          ProductId = prod.Id,
        //          ProductNumber = prod.ProductNumber,
        //          Quantity = prod.Quantity,
        //          ProductStandartCost = prod.StandartCost,
        //          ProductTypeId = prod.ProductTypeId,
        //          Manufacturer = prod.Manufacturer,
        //          Description = prod.Description,
        //          Parameters = prod.ProductParameters
        //            .Select(par => new ParameterDTO
        //            {
        //                Id = par.Id,
        //                ParameterId = par.ParameterId,
        //                Value = par.Value,
        //                Name = par.Parameter.Name
        //            })

        //            .ToList()
        //      })

        //      .ToListAsync();


        //    return products;
        //}

        //public async Task<IEnumerable<ProductOrderDTO>> GetParamForOrder(int id)
        //{
        //    var param = await _context.Products
        //                              .Where(p => p.Id == id)
        //                              .Select(m => new ProductOrderDTO
        //                              {
        //                                  ProductNumber = m.ProductNumber,
        //                                  ProductStandartCost = m.StandartCost
        //                              }).ToListAsync();

        //    return param;

        //}
        private void ValidateAddOtherProducts(addNewProductTypeRequest product)
        {
            if (string.IsNullOrEmpty(product.Name?.Value))
                throw new ApplicationException("Enter valid name product");

            if (string.IsNullOrEmpty(product.WareHouseName?.Value))
                throw new ApplicationException("Enter valid name warehouse");
        }

        private int GetOrCreateWarehouse(string warehouseName)
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

        private int GetOrCreateCurrency(string currencyName)
        {
            var currency = _context.Currencies.FirstOrDefault(c => c.CurrencyName == currencyName);
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

        private int GetOrCreateProductType(string productTypeName)
        {
            var productType = _context.ProductTypes.FirstOrDefault(pt => pt.NameType == productTypeName);
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

        private int GetOrCreateProductTypeHistory(string productTypeName, int deletedProductTypeId)
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
        private int GetOrCreateParameterHistory(int parameterId, int productTypeHistoryId)
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
                    Priority = parameter.Priority
                };

                _context.ParameterHistory.Add(newParameter);
                _context.SaveChanges();
                idParameter = newParameter.Id;
            }
            else
                idParameter = parameterH.Id;

            return idParameter;
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