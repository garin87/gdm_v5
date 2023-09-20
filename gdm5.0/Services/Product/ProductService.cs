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


using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.Linq.Expressions;
using gdm5._0.Domain.Models.ProductParameter;

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
                {
                    ProductId = product.Id,
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
                CurrencyId = currencyId,
                LastEditedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name
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
                CurrencyId = currencyId,
                LastEditedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name
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

        public List<ProductParametrDTO> getInstancesOfProductParameter(getInstancesOfProductParameterRequest requestParameters)
        {

            var productType = _context.ProductTypes.Where(type => type.NameType == requestParameters.NameType.Trim()).FirstOrDefault();

            if (productType == null)
                throw new ApplicationException("Product name " + requestParameters.NameType + " does not exist");


            SortOptionsDTO parametrOption = new SortOptionsDTO()
            {
                IsParameter = false
            };
            parametrOption.Name = requestParameters.NameParameter.Trim().ToLower();
            parametrOption.IsParameter = requestParameters.IsParameter;
            var productParameters = new List<ProductParameter>();
            var parameters = new List<Parameter>();
            List<dynamic> parameterValues = new List<dynamic>();
            if (requestParameters.IsParameter)
            {
                var paramId = _context.Parameters.Where(param => param.ProductTypeId == productType.Id)
                                                 .Where(el => el.Name.Trim().ToLower() == parametrOption.Name).FirstOrDefault()?.Id;

                if (paramId != null)
                {
                    parameterValues = _context.ProductParameters.Where(type => type.ParameterId == paramId && !string.IsNullOrEmpty(type.Value))
                                                                .Select(field => field.Value as dynamic).Distinct().ToList();
                }
            }
            else
            {
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
                    {
                        double orgValue = 0;
                        double parsedValue = 0;
                        if (double.TryParse(value, out parsedValue))
                        {
                            orgValue = parsedValue;
                        }

                        PproductParameters.Add(new ProductParametrDTO { Name = requestParameters.NameParameter, Value = value, ValueDouble = orgValue});
                    }
                        
                };
            }

            if (PproductParameters.FirstOrDefault().ValueDouble != 0)
            {
                return PproductParameters.OrderBy(el => el.ValueDouble).ToList<ProductParametrDTO>();
            }
            return PproductParameters.OrderBy(el => el.Value).ToList<ProductParametrDTO>();
        }
        //public List<ProductParametrDTO> getParametersByName(getInstancesOfProductParameterRequest requestParameters)
        //{
        //    var productType = getProductTypeByName(requestParameters.NameType);

        //    List<ProductParametrDTO> productParameters = GetProductParameters(productType, requestParameters);

        //    return OrderProductParameters(productParameters);
        //}

        public List<ProductParametrDTO> GetInstancesOfProductParameterUpdated(getInstancesOfProductParameterRequest requestParameters)
        {
            var productType = getProductTypeByName(requestParameters.NameType);
        
            List<ProductParametrDTO> productParameters = GetProductParameters(productType, requestParameters);

            return OrderProductParameters(productParameters);
        }


  
        private ProductType getProductTypeByName(string typeName)
        {
            var productType = _context.ProductTypes.FirstOrDefault(type => type.NameType.Trim() == typeName.Trim());

            if (productType == null)
            {
                throw new ArgumentException("Product name " + typeName + " does not exist");
            }

            return productType;
        }

        private List<ProductParametrDTO> GetProductParameters(ProductType productType, getInstancesOfProductParameterRequest parametrOption)
        {

            var parameterValues = parametrOption.IsParameter
                ? GetParameterValues(productType, parametrOption)
                : GetProductPropertyValues(productType, parametrOption);

            return parameterValues
                .Select(value => new ProductParametrDTO
                {
                    Name = parametrOption.NameParameter,
                    Value = value,
                    ValueDouble = TryParseDouble(value)
                })
                .ToList();
        }

        private List<string> GetParameterValues(ProductType productType, getInstancesOfProductParameterRequest parametrOption)
        {
            var param = _context.Parameters
                  .Where(param => param.ProductTypeId == productType.Id && param.Name.Trim().ToLower() == parametrOption.NameParameter)
                 // .Select(param => (int?)param.Id) // need to improve
                  .FirstOrDefault();

            var pProducts = param != null
                 ? _context.ProductParameters
                     .Where(productParam => productParam.ParameterId == param.Id && !string.IsNullOrEmpty(productParam.Value))
                     .ToList() : null;


            if (parametrOption.FilterParameters.Count() > 0)
            {
                // Get parameter info
                List<ParamaterValue> listParameters  = new List<ParamaterValue>();
                foreach (var fParam in parametrOption.FilterParameters)
                {
                    var p = _context.Parameters
                     .Where(parameter => parameter.ProductTypeId == productType.Id && parameter.Name.Trim().ToLower() == fParam.ParameterName)
                     .FirstOrDefault();
                   
                    if (p != null)
                    {
                        listParameters.Add(new ParamaterValue() { ParameterValue = fParam.ParameterValue, SelectedParameter = p });
                    }

                }
               
                if(listParameters != null && listParameters.Count()>0)
                {
                    listParameters.OrderBy(p => p.SelectedParameter.Priority);
                    var firstParameter = listParameters.First();
                    var topPParameterValue = _context.ProductParameters
                       .Where(productParam => productParam.ParameterId == firstParameter.SelectedParameter.Id && !string.IsNullOrEmpty(productParam.Value) 
                             && productParam.Value.Equals(firstParameter.ParameterValue))
                       .ToList();

                    // List<ProductParameter> filtersProduct = null;
                    listParameters.Remove(firstParameter);
                    foreach (var fparam in listParameters)
                    {
                       var secPProducts =_context.ProductParameters
                       .Where(productParam => productParam.ParameterId == fparam.SelectedParameter.Id && !string.IsNullOrEmpty(productParam.Value)
                             && productParam.Value.Equals(fparam.ParameterValue))
                       .ToList();
                      
                        List<ProductParameter> fValues = new List<ProductParameter>();
                        foreach (var tr in topPParameterValue)
                        {
                            foreach (var secPP in secPProducts)
                            {
                                if (tr.ProductId == secPP.ProductId)
                                {
                                    fValues.Add(tr);
                                }

                            }
                        }
                        topPParameterValue = fValues;
                        //     var filterdProducts = ;
                        //   filtersProduct.AddRange(filterdProducts);
                    }

                    if (topPParameterValue.Count() > 0)
                    {
                        List<ProductParameter> fValues = new List<ProductParameter>();
                        foreach (var tr in pProducts)
                        {
                            foreach (var topPP in topPParameterValue)
                            {
                                if (tr.ProductId == topPP.ProductId)
                                {
                                    fValues.Add(tr);
                                }

                            }
                        }

                        var paramValues = fValues.Select(pp => pp.Value).Distinct().ToList();
                        if (param != null && ((param.Name.ToLower()).Equals(("диаметр")) || (param.Name.ToLower()).Equals(("размер"))))
                        {
                            return sortDiamterParameterValues(paramValues); // sort string with delimiter *
                        }

                        return paramValues;
                    }
                }

            }
            var pValues = pProducts.Count() > 0 ? pProducts.Select(pp => pp.Value).Distinct().ToList() : new List<string>();
            if (param != null && ((param.Name.ToLower()).Equals(("диаметр")) || (param.Name.ToLower()).Equals(("размер"))))
            {
                return sortDiamterParameterValues(pValues); // sort string with delimiter *
            }

            return pValues;
        }

        private List<string> sortDiamterParameterValues(List<string> pValues)
        {
            double parsedValue = 0;
            Dictionary<string, double> listDiamters = new Dictionary<string, double>();
            foreach (var item in pValues)
            {
                if (double.TryParse(item, out parsedValue))
                {
                    listDiamters.Add(item, parsedValue);
                }
                else
                {
                    string[] parts = item.Split('*');
                    if (parts.Length > 0)
                    {
                        if (double.TryParse(parts[0], out double number))
                        {
                            listDiamters.Add(item, number);
                        }
                    }


                    parts = item.Split('/');
                    if (parts.Length > 0)
                    {
                        if (double.TryParse(parts[0], out double number))
                        {
                            listDiamters.Add(item, number);
                        }
                    }
                }

            }
            var rr = listDiamters.OrderBy(item => item.Value);
            return rr.Select(item => item.Key).ToList();
        }

        private List<string> GetParameterValues32(ProductType productType, getInstancesOfProductParameterRequest parametrOption)
        {
            var parameterId = _context.Parameters
                .Where(param => param.ProductTypeId == productType.Id && param.Name.Trim().ToLower() == parametrOption.NameParameter)
                .Select(param => param.Id)
                .FirstOrDefault();

            if (parameterId != 0)
                return new List<string>();

            var productParameters = _context.ProductParameters
                .Where(productParam => productParam.ParameterId == parameterId && !string.IsNullOrEmpty(productParam.Value))
                .ToList();

            if (parametrOption.FilterParameters.Count() == 0)
                return productParameters.Select(pp => pp.Value).Distinct().ToList();

            var filterParameterValues = new List<ParamaterValue>();

            foreach (var filterParam in parametrOption.FilterParameters)
            {
                var parameter = _context.Parameters
                    .Where(param => param.ProductTypeId == productType.Id && param.Name.Trim().ToLower() == filterParam.ParameterName)
                    .FirstOrDefault();

                if (parameter != null)
                {
                    filterParameterValues.Add(new ParamaterValue { ParameterValue = filterParam.ParameterValue, SelectedParameter = parameter });
                }
            }

            filterParameterValues = filterParameterValues.OrderBy(p => p.SelectedParameter.Priority).ToList();

            var topParameter = filterParameterValues.First();
            var topParameterValues = _context.ProductParameters
                .Where(productParam => productParam.ParameterId == topParameter.SelectedParameter.Id &&
                                       !string.IsNullOrEmpty(productParam.Value) &&
                                       productParam.Value.Equals(topParameter.ParameterValue))
                .ToList();

            foreach (var parameter in filterParameterValues.Skip(1))
            {
                var secondaryParameterValues = _context.ProductParameters
                    .Where(productParam => productParam.ParameterId == parameter.SelectedParameter.Id &&
                                           !string.IsNullOrEmpty(productParam.Value) &&
                                           productParam.Value.Equals(parameter.ParameterValue))
                    .ToList();

                topParameterValues = topParameterValues
                    .Join(secondaryParameterValues, tv => tv.ProductId, sv => sv.ProductId, (tv, sv) => tv)
                    .ToList();
            }

            var finalValues = productParameters
                .Join(topParameterValues, pp => pp.ProductId, tv => tv.ProductId, (pp, tv) => pp.Value)
                .Distinct()
                .ToList();

            return finalValues;
        }
        private List<string> GetProductPropertyValues(ProductType productType, getInstancesOfProductParameterRequest parametrOption)
        {

            var x = Expression.Parameter(typeof(Product), parametrOption.NameParameter);
            var body = Expression.PropertyOrField(x, parametrOption.NameParameter);
            var lambda = Expression.Lambda<Func<Product, string>>(body, x);

            var tt = _context.Products
                .Where(param => param.ProductTypeId == productType.Id)
                .Select(el => lambda.Compile())
                .Select(el => el.ToString())
                .Distinct()
                .ToList();

            return tt;
        }

        //    public dynamic GetSortField(SortOptionsDTO sortOption, List<Parameter> parameters)
        //    {
        //        if (SortFieldMappings.TryGetValue(sortOption.Name, out Func<Product, dynamic> selector))
        //        {
        //            return selector(this);
        //        }

        //        if (sortOption.IsParameter)
        //        {
        //            return GetParameterSortField(sortOption.Name, parameters);
        //        }

        //        return this.Id;
        //    }


        private double TryParseDouble(string value)
        {
            double orgValue = 0;
            if (double.TryParse(value, out orgValue))
            {
                return orgValue;
            };
            return orgValue;
        }

        private static readonly Dictionary<string, Func<Product, string>> SortFieldMappings = new Dictionary<string, Func<Product, string>>
        {
           { "quantity", p => p.Quantity.ToString() },
           { "productnumber", p => p.ProductNumber },
           { "manufacturer", p => p.Manufacturer },
           { "standartcost", p => p.StandartCost.ToString() },
           { "primecost", p => p.PrimeCost.ToString() },
           { "primecostusd", p => p.PrimeCostUSD.ToString() },
           { "primecosteur", p => p.PrimeCostEUR.ToString() },
           { "dateofreceipt", p => p.DateOfReceipt.ToString() },
           { "warehousename", p => p.Manufacturer }
        };

        private List<ProductParametrDTO> OrderProductParameters(List<ProductParametrDTO> productParameters)
        {
            return productParameters
                .OrderBy(el => el.ValueDouble != 0 ? el.ValueDouble : double.MaxValue)
                .ToList();
        }

        private dynamic parseStringValue(string orgValue)
        {
            double parsedValue = 0;
            if (double.TryParse(orgValue, out parsedValue))
            {
                return parsedValue;
            }

            return orgValue;
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


            //var currency = _context.Currencies.FirstOrDefault(currency => currency.CurrencyName == productDTO.currency.Value);
            //var currencyId = 0;

            //if (currency != null)
            //{
            //    currencyId = currency.Id;
            //}
            //else throw new ApplicationException("Entered name of currency does not exist");

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

            if (PrimeCostUSD != 0)
            {
                product.PrimeCostUSD = PrimeCostUSD;
            }
            if (PrimeCostEUR != 0)
            {
                product.PrimeCostEUR = PrimeCostEUR;
            }
            product.Description = productDTO.description?.Value;
            product.DateOfReceipt = DateOfReceipt;
            product.WareHouseId = idWarehouse;
          //  product.CurrencyId = currencyId;
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
                CurrencyId = product.CurrencyId ?? 0,
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
                CurrencyId = product.CurrencyId ?? 0,
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
        public static void CallParseFloat(string valueText, out float dd)
        {
            if (!String.IsNullOrEmpty(valueText))
            {
                float d;
                bool result = float.TryParse(valueText, out d);

                if (result)
                    dd = d;
                else
                    dd = 0;
            }
            else dd = 0;
        }
        public byte[] GeneratePDF()
        {
            PdfDocument document = new PdfDocument();
            //You will have to add Page in PDF Document
            PdfPage page = document.AddPage();
            //For drawing in PDF Page you will nedd XGraphics Object
            XGraphics gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
            //For Test you will have to define font to be used
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
            //Finally use XGraphics & font object to draw text in PDF Page
            gfx.DrawString("My First PDF Document", font, XBrushes.Black,
            new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
            //Specify file name of the PDF file
            string filename = "FirstPDFDocument.pdf";
            //Save PDF File
            document.Save(filename);
            //Load PDF File for viewing
         //   Process.Start(filename);

            // Send PDF to browser
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }

        void DefineStyles(PdfDocument document)
        {
            // Get the predefined style Normal.
          //  Style style =  document.["Normal"];
            //// Because all styles are derived from Normal, the next line changes the 
            //// font of the whole document. Or, more exactly, it changes the font of
            //// all styles and paragraphs that do not redefine the font.
            //style.Font.Name = "Verdana";

            //style = document.Styles[StyleNames.Header];
            //style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            //style = document.Styles[StyleNames.Footer];
            //style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            //// Create a new style called Table based on style Normal
            //style = document.Styles.AddStyle("Table", "Normal");
            //style.Font.Name = "Verdana";
            //style.Font.Name = "Times New Roman";
            //style.Font.Size = 9;

            //// Create a new style called Reference based on style Normal
            //style = document.Styles.AddStyle("Reference", "Normal");
            //style.ParagraphFormat.SpaceBefore = "5mm";
            //style.ParagraphFormat.SpaceAfter = "5mm";
            //style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }




    }


}

