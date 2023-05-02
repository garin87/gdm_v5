using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.Services;
using gdm5._0.Services.Interfaces;
using gdm5._0.DTO;
using gdm5._0.Helpers;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using gdm5._0.Domain.Models.Filters;
using gdm5._0.Filters;
using gdm5._0.Extensions;

namespace gdm5._0.Services
{
    public class ProductTypeService : BaseService<ProductType>, IProductTypeService
    {
        private readonly DataContext _context;
        private readonly IUriService _uriService;
        public ProductTypeService(DataContext context, IUriService uriService,
            IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            this._uriService = uriService;
        }

        public List<ProductParametrDTO> getProductTypeParameters(string nameType)
        {
            var productType = _context.ProductTypes.Where(type => type.NameType == nameType).FirstOrDefault();

            if (productType == null)
                throw new ApplicationException("Product name " + nameType + " does not exist");

            var parametrs = _context.Parameters.Where(parameter => parameter.ProductTypeId == productType.Id).Distinct();
            Console.WriteLine(parametrs);
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
                        NameType = param.NameType
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
            if (string.IsNullOrEmpty(nameProductType))
                throw new ApplicationException("Enter valid name product");

            var productType = _context.ProductTypes.Where(type => type.NameType == nameProductType).FirstOrDefault();

            if (productType == null)
                throw new ApplicationException("Product name " + nameProductType + " does not exist");


            var parameters = _context.Parameters.Where(param => param.ProductTypeId == productType.Id);

            var isSortActive = string.IsNullOrEmpty(sortOption.Name) && string.IsNullOrEmpty(sortOption.Direction);

            filter.StartIndex = validFilter.PageNumber - 1;
            filter.CountInstances = validFilter.PageSize;
            filter.ProductTypeId = productType.Id;
            var filterAssigner = new ProductAssigner(filter);
            var instancesOfProduct = _context.Products.AsNoTracking()
                                         .OrderByDescending(product => product.DateOfReceipt)
                                         .Include(product => product.ProductParameters)
                                         .Include(product => product.ProductType)
                                         .Include(product => product.PriceListValue)
                                         .Include(product => product.WareHouse)
                                         .Include(product => product.Currency)
                                         .ApplyPagingFilter(filterAssigner);


            if (!isSortActive)
            {
                var parametrs = new List<Parameter>();

                if (sortOption.IsParameter)
                {
                    parametrs = _context.Parameters.Where(param => param.ProductTypeId == productType.Id).ToList();
                    //  var paramId = parametrs.Where(el => el.Name == sortOption.Name).FirstOrDefault().Id;
                    //   productParameters = _context.ProductParameters.Where(type => type.ParameterId == paramId).ToList();
                    // productParameters = productParameters.Where(el => !string.IsNullOrEmpty(el.Value)).ToList();
                }


                instancesOfProduct = this.getSortProducts(instancesOfProduct, parametrs, sortOption);
            }

            var items = new List<ProductDTO>();

            foreach (var product in instancesOfProduct)
            {
                var productDTO = new ProductDTO()
                {
                    ProductId = product.Id,
                    ProductNumber = product.ProductNumber,
                    Manufacturer = product.Manufacturer,
                    Supplier = product.Supplier,
                    Quantity = product?.Quantity,
                    ProductStandartCost = product?.StandartCost,
                    ProductTypeId = product.ProductTypeId,
                    NameType = product.ProductType.NameType,
                    Description = product.Description,
                    DateOfReceipt = product?.DateOfReceipt.ToString("MM/dd/yyyy HH:mm"),
                    DateOfLastChanged = product.DateOfLastChanged.ToString("MM/dd/yyyy HH:mm"),
                    LastEditedByUser = product.LastEditedByUser,
                    Currency = product.Currency?.CurrencyName,
                    WareHouse = product.WareHouse?.Name,
                    PrimeCost = product?.PrimeCost,
                    PrimeCostEUR = product?.PrimeCostEUR,
                    PrimeCostUSD = product?.PrimeCostUSD,
                    StandartCost = product?.StandartCost,
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


            var totalRecords = _context.Products.Where(t => t.ProductTypeId == productType.Id).Count();
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


    }


}
