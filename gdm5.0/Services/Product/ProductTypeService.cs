using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using gdm5._0.DTO;
using gdm5._0.Helpers;
using gdm5._0.Domain.Models.Filters;
using gdm5._0.Filters;
using gdm5._0.Extensions;
using gdm5._0.Shared;
using gdm5._0.Domain.Models.Order;
using gdm5._0.Requests.Product;
using gdm5._0.Domain.Models.Product;

namespace gdm5._0.Services
{
    public class ProductTypeService : BaseService<ProductType>, IProductTypeService
    {
        private readonly IUriService _uriService;
        public ProductTypeService(DataContext context, IUriService uriService) : base(context)
        {
            _uriService = uriService;
        }

        public List<ProductParametrDTO> getProductTypeParameters(string nameType)
        {
            var productTypeID = GetProductTypeID(nameType);
            var parametrs = _context.Parameters.Where(parameter => parameter.ProductTypeId == productTypeID).Distinct();
        
            List<ProductParametrDTO> ProductParameters = new List<ProductParametrDTO>();
            if (parametrs != null)
            {
                foreach (var param in parametrs)
                {
                    ProductParameters.Add(new ProductParametrDTO
                    {
                        Id = param.Id,
                        Value = param.Name,
                        Priority = param.Priority,
                        NameType = param.NameType,
                        isRequired = param.isRequired
                    });
                };
            }


            return ProductParameters;
        }
        public PagedResponseDTO<List<ProductDTO>> getProductTypeInstances(string nameProductType,
            PaginationFilterDTO pageFilter, string route, SortOptionsDTO sortOption,
            ProductFilter filter)
        {
            var validFilter = new PaginationFilterDTO(pageFilter.PageNumber, pageFilter.PageSize);
            var productTypeID = GetProductTypeID(nameProductType);
            var parameters = GetParametersOfProduct(productTypeID);
            var isSortActive = string.IsNullOrEmpty(sortOption.Name) && string.IsNullOrEmpty(sortOption.Direction);

            if(filter.Parameters is Array)
            {
                foreach (var param in filter.Parameters)
                {
                    int? id = parameters.FirstOrDefault(p => p.Name.Equals(param.ParameterName))?.Id ;

                    param.ParameterId = id ?? 0;
                }
            }


            filter.StartIndex = validFilter.PageNumber - 1;
            filter.CountInstances = validFilter.PageSize;
            filter.ProductTypeId = productTypeID;
            var filterAssigner = new ProductAssigner(filter);

            var instancesOfProduct = _context.Products
                                             .OrderByDescending(product => product.DateOfReceipt)
                                             .Include(product => product.ProductParameters)
                                             .Include(product => product.ProductType)
                                             .Include(product => product.WareHouse)
                                             .Include(product => product.Currency)
                                             .AsNoTracking()
                                             .ApplyFilter(filterAssigner);
                                             
            var testQueryString = instancesOfProduct.ToQueryString();

            var totalRecords = GlobalVariables.TotalRecords;
            if (!isSortActive)
            {
                instancesOfProduct = getSortProducts(instancesOfProduct, parameters.ToList(), sortOption);
            }
         
            instancesOfProduct = filter.CountInstances > 0 ? instancesOfProduct.Skip(filter.StartIndex).Take(filter.CountInstances): instancesOfProduct;

            var items = new List<ProductDTO>();
            foreach (var product in instancesOfProduct)
            {
                var priceListValue = _context.PriceListValues.FirstOrDefault(pv => pv.ProductId == product.Id);
                var priceListValueEUR = priceListValue?.PriceEUR ?? 0;
                var priceListValueEURNDS = priceListValue?.PriceEURNDS ?? 0;
                var productDTO = new ProductDTO()
                {
                    ProductId = product.Id,
                    ProductNumber = product.ProductNumber,
                    Manufacturer = product.Manufacturer,
                    Supplier = product.Supplier,
                    Quantity = Math.Round((double)(product?.Quantity), 2),
                    ProductStandartCost = product?.StandartCost,
                    ProductTypeId = product.ProductTypeId,
                    NameType = product.ProductType.NameType,
                    Description = product.Description,
                    DateOfReceipt = product?.DateOfReceipt.ToString("MM/dd/yyyy HH:mm"),
                    DateOfLastChanged = product.DateOfLastChanged.ToString("MM/dd/yyyy HH:mm"),
                    LastEditedByUser = product.LastEditedByUser,
                    Currency = product.Currency?.CurrencyName,
                    WareHouse = product.WareHouse?.Name,
                    PrimeCost = (double)Math.Round((double)(product?.PrimeCost), 2),
                    PrimeCostEUR = (double)Math.Round((double)(product?.PrimeCostEUR), 2),
                    PrimeCostUSD = (double)Math.Round((double)(product?.PrimeCostUSD), 2),
                    StandartCost = (double)Math.Round((double)(product?.PrimeCostEUR), 2), // need to improve

                    PriceValueEUR = (double)Math.Round((double)(priceListValueEUR), 2),
                    PriceValueEURNDS = (double)Math.Round((double)(priceListValueEURNDS), 2),
                    Name = product.Name,

                };

                foreach (var typeParam in parameters)
                {
                    var paramDTO = new ParameterDTO();
                    var value = product.ProductParameters.FirstOrDefault(t => t.ParameterId == typeParam.Id);
                    if (value != null)
                    {
                        paramDTO.Id = value.Id;
                        paramDTO.Value = value.Value;
                        paramDTO.Priority = typeParam.Priority;
                        paramDTO.NameType = typeParam.NameType;
                    }

                    paramDTO.ParameterId = typeParam.Id;
                    paramDTO.Name = typeParam.Name;

                    productDTO.Parameters.Add(paramDTO);

                }

                items.Add(productDTO);
            }
          
            if (GlobalVariables.TotalRecords == 0)
            {
                totalRecords = _context.Products.Where(t => t.ProductTypeId == productTypeID).Count();
            }

          //  var sserRole = this._httpContextAccessor.HttpContext.User.Claims.FirstOrDefault()?.Value;
            var productReponse = PaginationHelper.CreatePagedReponse<ProductDTO>(items, validFilter, totalRecords, _uriService, route);
           
            return productReponse;
        }
        private IQueryable<Product> getSortProducts(
              IQueryable<Product> products, List<Parameter> parameters, SortOptionsDTO sortOption)
        {
            var productsT = products.AsEnumerable();
            if (sortOption.Direction.Trim() == "asc")
                return productsT.OrderBy(product => product.getSortField(sortOption, parameters)).AsQueryable();
            else
            {
                return productsT.OrderByDescending(product => product.getSortField(sortOption, parameters)).AsQueryable();
            }
        }
        public List<ProductTotalQuantity> LoadProdutReport(loadProductReportRequest request)
        {
            int productTypeID = GetProductTypeID(request.Name?.Value);
            int parameterDiameterID = GetParameterDiameterID(productTypeID);

            ProductFilter filter = new ProductFilter()
            {
                Parameters = request.Parameters,
                Manufacturer = request.Manufacturer?.Value,
            };

            var parameters = GetParametersOfProduct(productTypeID);

            if (filter.Parameters is Array)
            {
                foreach (var param in filter.Parameters)
                {
                    int? id = parameters.FirstOrDefault(p => p.Name.Equals(param.Name) && p.ProductTypeId == productTypeID)?.Id;

                    param.ParameterId = id ?? 0;
                }
            }


            var filterAssigner = new ProductAssigner(filter);

            var OrderProductTotalQ = getProductQuantityByParameter(productTypeID, parameterDiameterID, filterAssigner);
           
            var test11 = OrderProductTotalQ.ToQueryString();
            var test22 = OrderProductTotalQ.ToList();

            var groupedResults = OrderProductTotalQ
                .GroupBy(t => t.Diameter)
                .Select(g => new ProductTotalQuantity
                {
                    Name = request.Name.Value,
                    Diameter = g.Key,
                    TotalAmount = (double)Math.Round((double)(g.Sum(t => t.TotalAmount)), 2),
                })
                .OrderByDescending(t => t.TotalAmount)
                .ToList();

            return groupedResults;
        }
        private IQueryable<OrderTotalQuantity> getProductQuantityByParameter(int productTypeID, int parameterID,
            ProductAssigner filterProductAssigner)
        {
            return from subT in (
                           from p in _context.Products.ApplyFilter(filterProductAssigner) 
                           join pt in _context.ProductTypes on p.ProductTypeId equals pt.Id
                           join param in _context.Parameters on pt.Id equals param.ProductTypeId
                           join pp in _context.ProductParameters on param.Id equals pp.ParameterId
                           where pt.Id == productTypeID && pp.ParameterId == parameterID
                           select new {
                               Diameter = pp.Value,
                               ProductIDF = p.Id,
                               pp.ProductId,
                               pp.ParameterId,
                               Amount = p.Quantity,
                           })
                   join pp in _context.ProductParameters on subT.ProductIDF equals pp.ProductId
                   where subT.ProductIDF == subT.ProductId && pp.ParameterId == parameterID
                   group subT by subT.Diameter into g
                   select new OrderTotalQuantity
                   {
                       Diameter = g.Key,
                       TotalAmount = g.Sum(subT => subT.Amount)
                   };
        }
        private int GetParameterDiameterID(int productTypeId)
        {
            var parameterDiameter = _context.Parameters.FirstOrDefault(p => p.ProductTypeId == productTypeId &&
                                                                      (p.Name.ToLower().Equals("диаметр") ||
                                                                       p.Name.ToLower().Equals("размер") ||
                                                                       p.Name.ToLower().Equals("внутренний диаметр")));

            if (parameterDiameter == null)
                throw new ApplicationException("Product has no 'диаметр' or 'размер'");

            return parameterDiameter.Id;
        }
    }


}
