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

namespace gdm5._0.Services
{
    public class ProductTypeService : BaseService<ProductType>, IProductTypeService
    {
        private readonly DataContext _context;
        private readonly IUriService _uriService;
        public ProductTypeService(DataContext context, IUriService uriService) : base(context)
        {
            _context = context;
            this._uriService = uriService;
        }

        public List<ProductParametrDTO> getProductTypeParameters(string nameType)
        {
            var productType = _context.ProductTypes.Where(type => type.NameType == nameType).FirstOrDefault();

            if (productType == null)
                throw new ApplicationException("Product name " + nameType + " does not exist");

            var parametrs = _context.Parameters.Where(param => param.ProductTypeId == productType.Id);
            List<ProductParametrDTO> PproductParameters = new List<ProductParametrDTO>();
            if (parametrs != null)
            {
                foreach (var param in parametrs)
                {
                    PproductParameters.Add(new ProductParametrDTO { Id = param.Id, Value = param.Name });
                };
            }


            return PproductParameters;
        }

        public PagedResponseDTO<List<ProductDTO>> getProductTypeInstances(string nameProductType,
            PaginationFilterDTO filter, string route, SortOptionsDTO sortOption)
        {
            var validFilter = new PaginationFilterDTO(filter.PageNumber, filter.PageSize);


            if (string.IsNullOrEmpty(nameProductType))
                throw new ApplicationException("Enter valid name product");

            var productType = _context.ProductTypes.Where(type => type.NameType == nameProductType).FirstOrDefault();

            if (productType == null)
                throw new ApplicationException("Product name " + nameProductType + " does not exist");


            var parameters = _context.Parameters.Where(param => param.ProductTypeId == productType.Id);

            var isSortActive = string.IsNullOrEmpty(sortOption.Name) && string.IsNullOrEmpty(sortOption.Direction);

            List<Product> products = null;
            var instancesOfProduct = _context.Products.Include(product => product.ProductParameters)
                                         .Include(product => product.ProductType)
                                         .Include(product => product.PriceListValue)
                                         .Include(product => product.WareHouse)
                                         .Include(product => product.Currency)
                                         .Where(t => t.ProductTypeId == productType.Id);
          
            if (!isSortActive)
            {
                instancesOfProduct =  this.getSortProducts(instancesOfProduct, parameters, sortOption);
            }

            products = instancesOfProduct.Skip((validFilter.PageNumber - 1))
                                         .Take(validFilter.PageSize)
                                         .ToList();

            var items = new List<ProductDTO>();

            foreach (var product in products)
            {
                var productDTO = new ProductDTO()
                {
                    ProductId = product.Id,
                    ProductNumber = product.ProductNumber,
                    Manufacturer = product.Manufacturer,
                    Quantity = product?.Quantity,
                    ProductStandartCost = product?.StandartCost,
                    ProductTypeId = product.ProductTypeId,
                    NameType = product.ProductType.NameType,
                    Description = product.Description,
                    DateOfReceipt = product?.DateOfReceipt,
                    Currency = product.Currency?.CurrencyName,
                    WareHouse = product.WareHouse?.Name,
                    PrimeCost = product?.PrimeCost,
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
              IQueryable<Product> products, IQueryable<Parameter> parameters, SortOptionsDTO sortOption)
        {
            var productsT = products.AsEnumerable();

            if(sortOption.Direction.Trim() == "asc")
                return productsT.OrderBy(product => product.getSortField(sortOption, parameters)).AsQueryable();
            else
            {
                return productsT.OrderByDescending(product => product.getSortField(sortOption, parameters)).AsQueryable();
            }
        }

        //private static readonly Dictionary<string, dynamic> OrderPrduct =
        //        new Dictionary<string, dynamic>
        //{
        //         { "manufacturer", (Expression<Func<Product, string>>)(x => x.Manufacturer) },
        //         { "productNumber", (Expression<Func<Product, string>>)(x => x.ProductNumber) },
        //         { "quantity",  (Expression<Func<Product, double>>)(x => x.Quantity) },
        //         { "standartCost",   (Expression<Func<Product, double>>)(x => x.StandartCost) },
        //         { "primeCost",   (Expression<Func<Product, double>>)(x => x.PrimeCost) },
        //         { "dateOfReceipt",   (Expression<Func<Product, DateTime>>)(x => x.DateOfReceipt) },
        //};


    }


}
