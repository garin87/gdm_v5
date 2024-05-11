
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using gdm5._0.Domain.Models.Filters;
using gdm5._0.Extensions;
using gdm5._0.Filters;
using gdm5._0.Helpers;
using gdm5._0.Models;
using gdm5._0.Requests.PriceList;
using gdm5._0.Services.Interfaces;


namespace gdm5._0.Services
{
    public class PriceListValueService : BaseService<PriceListValue>, IPriceListValueService
    {
        public PriceListValueService(DataContext context) : base(context)
        {
        }

        public void AddPriceListValues(addPriceListValueRequest priceListValue)
        {
            // Validate input
            if (priceListValue == null || string.IsNullOrEmpty(priceListValue.PriceListName))
                throw new ArgumentNullException(nameof(priceListValue), "Enter valid price list values");

            // Check if the price list exists
            var priceList = GetPriceListByName(priceListValue.PriceListName);
            if (priceList == null)
                throw new ApplicationException("Entered price list does not exist.");

            var idTypeProduct = GetProductType(priceListValue.Name.Value);

            double price = MathHelper.ParseDouble(priceListValue.Price?.Value);
            double quantity = MathHelper.ParseDouble(priceListValue.Quantity?.Value);
            double version = MathHelper.ParseDouble(priceListValue.Version?.Value);
            double percentOfMarkup = MathHelper.ParseDouble(priceListValue.PercentOfMarkup?.Value);


            ProductFilter filter = CreateProductParametersFilter(priceListValue);
            var filterAssigner = new ProductAssigner(filter);

            var parameters = GetParametersOfProduct(idTypeProduct);
            if (filter.Parameters is Array)
            {
                foreach (var param in filter.Parameters)
                {
                    int? id = parameters.FirstOrDefault(p => p.Name.Equals(param.ParameterName))?.Id;

                    param.ParameterId = id ?? 0;
                }
            }

            // product get id's
            var products = _context.Products.AsNoTracking()
                                            .Where(Products => (Products.ProductTypeId == idTypeProduct))
                                            .Include(p => p.ProductParameters)
                                            .ApplyFilter(filterAssigner);


            var FilterStartDimension = parseFilterD(priceListValue.FilterStartDimension?.Value);
            var FilterStartDimensionInt = FilterStartDimension.ParseInt();
            var FilterEndDimension = parseFilterD(priceListValue.FilterEndDimension?.Value);
            var FilterEndDimensionInt = FilterEndDimension.ParseInt();

            var parameterDimension = _context.Parameters.FirstOrDefault(p => p.ProductTypeId == idTypeProduct &&
            (p.Name.ToLower() == "диаметр" || p.Name.ToLower() == "размер" ||  p.Name.ToLower().Equals("внутренний диаметр")));

            int idParameterDimension = 0;
            if (parameterDimension != null) 
                idParameterDimension = parameterDimension.Id;

            List<Product> filterProduct = new List<Product>();
            if (!string.IsNullOrEmpty(FilterStartDimension) && !string.IsNullOrEmpty(FilterEndDimension))
            {
                foreach (var product in products)
                {
                    var productParameters = _context.ProductParameters.Where(pp => pp.ProductId == product.Id);
                    foreach (var pp in productParameters)
                    {
                        if (pp.ParameterId == idParameterDimension)
                        {
                            if (!string.IsNullOrWhiteSpace(pp.Value))
                            {
                                var valueParameter = parseFilterD(pp.Value);
                                var valueParameterInt = valueParameter.ParseInt();

                                if ((FilterStartDimensionInt < valueParameterInt ||
                                     FilterStartDimensionInt == valueParameterInt))
                                {
                                    if (FilterEndDimensionInt > valueParameterInt ||
                                       FilterEndDimensionInt == valueParameterInt)
                                    {
                                        filterProduct.Add(product);
                                    }

                                }

                            }
                        }
                    }


                }
            }
            else
            {
                filterProduct = products.ToList();
            }
         

            if (products == null)
                throw new ApplicationException("Entered products does not exist.");
  
            var test = products.ToList();
            if (filterProduct.Count() > 0)
            {
                Random rnd = new Random();
                int randInt = rnd.Next(10000);
                var uniqCode = products.FirstOrDefault().Id + priceList.Id + randInt; // need to improve
                foreach (var product in filterProduct)
                {
                    var percentPriceEUR = percentOfMarkup > 0 ? (percentOfMarkup / 100 * product.PrimeCostEUR) + product.PrimeCostEUR : 0;

                    var ter = (percentOfMarkup / 100) * product.PrimeCostEUR;
                    var duplicateProduct = _context.PriceListValues.FirstOrDefault(pv => pv.ProductId == product.Id );//&& priceList.Id == pv.PriceListId

                    var numberParamId = _context.Parameters.FirstOrDefault(param => param.ProductTypeId == product.ProductTypeId &&
                    param.Name.ToLower().Equals("номер"))?.Id ?? 0;

                    var PParameters = _context.ProductParameters.Where(pp => pp.ProductId == product.Id && pp.ParameterId != numberParamId).Select(pv => pv.Value);
                    var pValues = String.Join(", ", PParameters);
                   
                    if (duplicateProduct != null)
                    {
                        throw new ApplicationException("Product " +  pValues + " is duplicate for this price list");
                    }

                    var instancePriceListValue = new PriceListValue
                    {
                        Version = version,
                        PercentOfMarkup = (int)percentOfMarkup,
                        Price = price,
                        PriceEUR = percentPriceEUR,
                        PriceNDS = (0.2 * price) + price, // need to improve NDS 20
                        PriceEURNDS = (0.2 * percentPriceEUR) + percentPriceEUR, // need to improve NDS 20
                        Quantity = quantity,
                        Unit = priceListValue.Unit?.Value, // nedd to improve
                        Description = priceListValue.Description?.Value,
                        ProductId = product.Id,
                        PriceListId = priceList.Id,
                        ProductParameterUniqCode = uniqCode,
                        ProductParameterValueCode = pValues
                    };

                    _context.PriceListValues.Add(instancePriceListValue);
                }

                priceList.LastModifiedDate = DateTimeHelper.DateTimeNowWithOffset();
                _context.SaveChanges();
            } else throw new ApplicationException("Entered product does not exist.");

        }

        private string parseFilterD(string filterValue)
        {
            char[] separatingStrings = { '*', '/' };
            if (!string.IsNullOrWhiteSpace(filterValue))
            {
                var fisrtPart = filterValue.Split(separatingStrings)[0];

                if (!string.IsNullOrWhiteSpace(fisrtPart))
                {
                    return fisrtPart;
                }
                return filterValue;
            }
            return filterValue;
        }

        private ProductFilter CreateProductParametersFilter(addPriceListValueRequest priceListValue)
        {
            // List<ProductParametersFilter> parameters = new List<ProductParametersFilter>();
            List<ProductParametersFilter> parameters = new List<ProductParametersFilter>();

            foreach (var parameter in priceListValue.Parameters)
            {
                if (string.IsNullOrEmpty(parameter.Name)) continue;

                ProductParametersFilter productParametersFilter = new ProductParametersFilter()
                {
                    ParameterName = parameter.Name,
                    Value = parameter.Value
                };
                parameters.Add(productParametersFilter);
            }

            ProductFilter filter = new ProductFilter()
            {
                Manufacturer = priceListValue.Manufacturer?.Value,
                Parameters = parameters.ToArray()
            };

            return filter;
        }

        private PriceList GetPriceListByName(string priceListName)
        {
            return _context.PriceLists.FirstOrDefault(priceList => priceList.Name == priceListName);
        }

        public void UpdatePriceListValues(addPriceListValueRequest addPriceListValue)
        {

            if (addPriceListValue == null)
                throw new ArgumentNullException(nameof(addPriceListValue),"Enter valid price list values");

            if (addPriceListValue.PriceListValueProductUniqCode == 0)
                throw new ApplicationException("Enter valid price list product uniq code");

            var priceValues = _context.PriceListValues.Where(PriceListValue => PriceListValue.ProductParameterUniqCode == addPriceListValue.PriceListValueProductUniqCode);
            if (priceValues == null)
                throw new ApplicationException("Entered product group price value does not exist.");


            double price = MathHelper.ParseDouble(addPriceListValue.Price?.Value);
            int percentOfMarkup = MathHelper.ParseInt(addPriceListValue.PercentOfMarkup?.Value);

            foreach (var priceV in priceValues)
            {
                var productCostEUR = _context.Products.FirstOrDefault(p => p.Id == priceV.Id)?.PrimeCostEUR;
                var percentPriceEUR = (double)percentOfMarkup > 0 ? ((double)percentOfMarkup / 100 * productCostEUR) + productCostEUR : 0;

                priceV.PriceEUR = (double)percentPriceEUR;
                priceV.PriceEURNDS = (double)((0.2 * percentPriceEUR) + percentPriceEUR);
                priceV.Price = price;
                priceV.PercentOfMarkup = percentOfMarkup;
            }

         
            _context.SaveChanges();

        }
        public void UpdatePriceListValueProduct(addPriceListValueRequest addPriceListValue)
        {

            if (addPriceListValue == null)
                throw new ArgumentNullException(nameof(addPriceListValue), "Enter valid price list values");

            if (addPriceListValue.ProductId == 0)
                throw new ApplicationException("Enter valid PriceListValue Id");

            var priceValues = _context.PriceListValues.Where(PriceListValue => PriceListValue.ProductId == addPriceListValue.ProductId);
            if (priceValues == null)
                throw new ApplicationException("Entered product price value does not exist.");


            double price = MathHelper.ParseDouble(addPriceListValue.Price?.Value);
            int percentOfMarkup = MathHelper.ParseInt(addPriceListValue.PercentOfMarkup?.Value);
            Random rnd = new Random();
            int randInt = rnd.Next(10000);
            var uniqCode = priceValues.FirstOrDefault().Id + randInt; // need to improve
            foreach (var priceV in priceValues)
            {
                var productCostEUR = _context.Products.FirstOrDefault(p => p.Id == priceV.Id)?.PrimeCostEUR;
                var percentPriceEUR = (double)percentOfMarkup > 0 ? ((double)percentOfMarkup / 100 * productCostEUR) + productCostEUR : 0;

                priceV.PriceEUR = (double)percentPriceEUR;
                priceV.PriceEURNDS = (double)((0.2 * percentPriceEUR) + percentPriceEUR);
                priceV.Price = price;
                priceV.PercentOfMarkup = percentOfMarkup;
                priceV.ProductParameterUniqCode = uniqCode;
            }


            _context.SaveChanges();

        }

        public List<PriceListValue> GetPriceListValues(int priceListId)
        {
            if (priceListId <= 0)
                throw new ArgumentException("Invalid price list id");

            var priceListValues = _context.PriceListValues
                .Where(p => p.PriceListId == priceListId)
                .ToList();

            return priceListValues;
        }

        public void DeletePriceListValues(int PriceListValueProductUniqCode)
        {
            if (PriceListValueProductUniqCode == 0)
                throw new ApplicationException("Enter valid price list product uniq code");

            var priceValues = _context.PriceListValues.Where(PriceListValue => 
                              PriceListValue.ProductParameterUniqCode == PriceListValueProductUniqCode);
            if (priceValues == null)
                throw new ApplicationException("Entered product group price value does not exist.");


            _context.PriceListValues.RemoveRange(priceValues);
            _context.SaveChanges();
        }
        private void ValidatePriceListValue(addPriceListValueRequest value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Enter valid price list value");

            if (value.ProductId == 0)
                throw new ApplicationException("Enter valid ProductId");

            var products = _context.Products.FirstOrDefault(Products => (Products.Id == value.ProductId));
            if (products == null)
                throw new ApplicationException("Entered product id does not exist.");
        }

        private int GetProductType(string productTypeName)
        {
            var productType = _context.ProductTypes.FirstOrDefault(pt => pt.NameType == productTypeName);

            if (productType == null)
            {
                throw new ApplicationException("Product " + productTypeName + " does not exist");    
            }

            return productType.Id;
        }

    }


}

