
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using gdm5._0.DTO;
using gdm5._0.Helpers;
using gdm5._0.Models;
using gdm5._0.Requests.PriceList;
using gdm5._0.Requests.Product;
using gdm5._0.Services.Interfaces;
using gdm5._0.Services.PriceListS;

namespace gdm5._0.Services
{
    public class PriceListService : PriceListBase<PriceList>, IPriceListService
    {
        public PriceListService(DataContext context) : base(context)
        {
        }

        public void AddPriceList(addPriceListRequest addPriceList)
        {
            ValidateName(addPriceList.Name?.Value);

            ValidateExistsPriceList(addPriceList.Name?.Value);

            double version = MathHelper.ParseDouble(addPriceList.Version?.Value);

            var compPLid = _context.ComplexPriceLists.First().Id;
            var instancePriceList = new PriceList
            {
                Name = addPriceList.Name?.Value,
                Version = version,
                Description = addPriceList.Description?.Value,
                CreationDate = DateTimeHelper.DateTimeNowWithOffset(),
                LastModifiedDate = DateTimeHelper.DateTimeNowWithOffset(),
                ComplexPriceListId = compPLid
            };

            _context.PriceLists.Add(instancePriceList);
            _context.SaveChanges();
        }
        public PriceListWithValues GetPriceList(string namePriceList)
        {
            var priceList = getPriceListByName(namePriceList);

            return new PriceListWithValues()
            {
                Id = priceList.Id,
                Name = priceList.Name,
                Description = priceList.Description,
                Version = priceList.Version,
                CreationDate = priceList.CreationDate.ToString("MM/dd/yyyy HH:mm"),
                LastModifiedDate = priceList.LastModifiedDate.ToString("MM/dd/yyyy HH:mm")
            };
        }
        public PriceListWithValues GetPriceListWithValues(string namePriceList)
        {
            var priceList = getPriceListByName(namePriceList);
            var priceListValues = getPriceListValues(priceList.Id);

            List<PriceListValueDTO> listPriceListValues = new List<PriceListValueDTO>();

            if (priceListValues.Any())
            {
                List<Double> uniqPL = new List<double>();
                Dictionary<string, Double> uniqPriceAndValue = new Dictionary<string, Double>();
                List<KeyValuePair<string, Double>> uniqPriceValues = new List<KeyValuePair<string, Double>>();


                foreach (var priceL in priceListValues)
                {
                    var produt = _context.Products.AsNoTracking()
                                                  .FirstOrDefault(product => product.Id == priceL.ProductId);
                    var produtParameter = _context.ProductParameters.Include(p => p.Parameter)
                                                                    .Where(pp => pp.ProductId == priceL.ProductId);

                    var pr = produtParameter.FirstOrDefault(p => p.Parameter.Name.ToLower().Equals("диаметр") ||
                                                                 p.Parameter.Name.ToLower().Equals("размер") ||
                                                                 p.Parameter.Name.ToLower().Equals("внутренний диаметр"));

                    var hasPrice = uniqPriceAndValue.GetValueOrDefault(pr?.Value + produt.PrimeCost.ToString(), 0);

                    if (hasPrice == 0)
                    {
                        var pv = new PriceListValueDTO()
                        {
                            Id = priceL.Id,
                            Version = priceL.Version,
                            PercentOfMarkup = priceL.PercentOfMarkup,
                            Price = Math.Round(priceL.Price, 2),
                            PriceEUR = Math.Round(priceL.PriceEUR, 2),
                            PriceNDS = Math.Round(priceL.PriceNDS, 2),
                            PriceEURNDS = Math.Round(priceL.PriceEURNDS, 2),
                            Quantity = priceL.Quantity,
                            Description = priceL.Description,
                            Unit = priceL.Unit,
                            ProductParameterUniqCode = priceL.ProductParameterUniqCode,

                            ProductId = priceL.ProductId,
                            PriceListId = priceL.PriceListId,

                            ProductName = produt.Name,
                            Manufacturer = produt.Manufacturer,
                        };

                        foreach (var prod in produtParameter)
                        {
                            var parameter = _context.Parameters.AsNoTracking().FirstOrDefault(param => param.Id == prod.ParameterId);
                            var pp = new ParameterDTO()
                            {
                                ParameterId = parameter.Id,
                                Name = parameter.Name,
                                Value = prod.Value,
                                Priority = parameter.Priority,
                                
                            };
                            pv.Parameters.Add(pp);
                        }

                        listPriceListValues.Add(pv);
                        var valueS = pv.Parameters.FirstOrDefault(pp => pp.Name.ToLower().Equals("диаметр") || 
                                                                        pp.Name.ToLower().Equals("размер") ||
                                                                        pp.Name.ToLower().Equals("внутренний диаметр")).Value;

                        uniqPriceAndValue.Add(valueS + produt.PrimeCost.ToString(), produt.PrimeCost);
                        uniqPriceValues.Add(new KeyValuePair<string, Double>(valueS, produt.PrimeCost));
                        uniqPL.Add(produt.PrimeCost);
                    }
                }
            }

            bool isSortActive = true; // need to improve
            if (isSortActive)
            {
                SortOptionsDTO SortOptions = new SortOptionsDTO()
                {
                    Direction = "asc",
                    IsParameter = true,
                    Name = ""
                };

                var parametrs = _context.Parameters.AsNoTracking().Select(p => p).ToList(); //new List<Parameter>();

                listPriceListValues = this.getSortProducts(listPriceListValues, parametrs, SortOptions);
            }


            var PL = new PriceListWithValues()
            {
                Id = priceList.Id,
                Name = priceList.Name,
                Description = priceList.Description,
                Version = priceList.Version,
                CreationDate = priceList.CreationDate.ToString("MM/dd/yyyy HH:mm"),
                LastModifiedDate = priceList.LastModifiedDate.ToString("MM/dd/yyyy HH:mm"),
                PriceListValue = listPriceListValues,

            };

            return PL;
        }
        public void UpdatePriceList(addPriceListRequest addPriceList)
        {
            var priceList = GetPriceListById(addPriceList?.idPriceList ?? 0);
            ValidateName(addPriceList.Name?.Value);

            double version = MathHelper.ParseDouble(addPriceList.Version?.Value);

            priceList.Name = addPriceList.Name?.Value;
            priceList.Version = version;
            priceList.Description = addPriceList.Description?.Value;
            priceList.LastModifiedDate = DateTimeHelper.DateTimeNowWithOffset();

            _context.SaveChanges();
        }
        public void DeletePriceList(int priceListId)
        {
            var priceListValuesToDelete = GetPriceListById(priceListId);
            var priceListValue = _context.PriceListValues.Where(pl => pl.PriceListId == priceListId).ToList();

            if(priceListValue != null)
            {
                _context.PriceListValues.RemoveRange(priceListValue);
            }
           
            _context.PriceLists.Remove(priceListValuesToDelete as PriceList);
            _context.SaveChanges();
        }
        private List<PriceListValueDTO> getSortProducts( List<PriceListValueDTO> priceValues, List<Parameter> parameters, SortOptionsDTO sortOption)
        {
            var priceValuesT = priceValues.AsEnumerable();
            if (sortOption.Direction.Trim() == "asc")
                return priceValuesT.OrderBy(priceListValue => priceListValue.getSortField(sortOption, parameters)).ToList();
            else
            {
                return priceValuesT.OrderByDescending(priceListValue => priceListValue.getSortField(sortOption, parameters)).ToList();
            }
        }
    }

}

