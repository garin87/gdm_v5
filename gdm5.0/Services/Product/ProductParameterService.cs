using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.DTO;
using gdm5._0.Services;
using gdm5._0.Services.Interfaces;

namespace gdm5._0.Services
{
    public class ProductParameterService : BaseService<ProductParameter>, IProductParameterService
    {
        private readonly DataContext _context;

        public ProductParameterService(DataContext context): base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductParameter>> GetProductParameters(long id)
        {
            var value = await _context.ProductParameters
                                      .Where(p => p.ParameterId == id)    
                                      .GroupBy(m => new { m.ParameterId, m.Value })
                                      .Select(group => group.FirstOrDefault())
                                      .Distinct()
                                      .ToListAsync();
            return value;
        }

        public async Task<IEnumerable<DiameterDTO>> GetProductDiameters(int id, string param, int paramId)
        {
            var value = await _context.ProductParameters.Where(pp1 => pp1.Value == param && pp1.ParameterId == paramId)
                        .Join(_context.ProductParameters.Where(pp => pp.ParameterId == id),
                                pp1 => pp1.Product.Id,
                                pp => pp.Product.Id,
                               (pp1, pp) => new DiameterDTO { 
                                    DiameterValue = pp.Value,
                                    DiameterId = pp.ParameterId,
                                    Param = param,
                                    ParamId = paramId,
                                    ProductTypeId  = pp.Parameter.ProductTypeId
                               })
                               .Distinct()
                               .ToListAsync();
            return value;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByDiameter(int typeId, string param, int paramId, 
                                                                         int paramDiameterId,  string diameter)
        {
            var products = await _context.ProductParameters
                .Include(p => p.Product)
                .Where(prod => prod.Product.ProductTypeId == typeId)
                .Where(pp1 => pp1.ParameterId == paramId && pp1.Value == param)
                .Join(_context.ProductParameters.Where(pp => pp.ParameterId == paramDiameterId && pp.Value == diameter),
                                  pp1 => pp1.Product.Id,
                                  pp => pp.Product.Id,
                                  (pp1, pp) => new ProductDTO
                                  {
                                      ProductId = pp.Product.Id,
                                      NameType = pp.Product.ProductType.NameType,
                                      ProductNumber = pp.Product.ProductNumber,
                                      Quantity = pp.Product.Quantity,
                                      ProductStandartCost = pp.Product.StandartCost,
                                      ProductTypeId = pp.Product.ProductTypeId,
                                      Manufacturer = pp.Product.Manufacturer,
                                      Description = pp.Product.Description,
                                      Parameters = pp.Product.ProductParameters
                                   .Select(par => new ParameterDTO
                                   {
                                       Id = par.Id,
                                       ParameterId = par.ParameterId,
                                       Value = par.Value,
                                       Name = par.Parameter.Name
                                   })
                                   .ToList()
                                  }).OrderBy(p => p.Quantity == 0).ThenBy(p => p.Quantity)
                                .ToListAsync();

            return products;
        }

        // Get Other Products
        public async Task<IEnumerable<ProductDTO>> GetOtherProducts(int typeId)
        {
            var products = await _context.ProductParameters
                .Include(p => p.Product)
                .Where(prod => prod.Product.ProductTypeId == typeId)
                .Select( pp => new ProductDTO
                {
                    ProductId = pp.Product.Id,
                    NameType = pp.Product.ProductType.NameType,
                    ProductNumber = pp.Product.ProductNumber,
                    Quantity = pp.Product.Quantity,
                    ProductStandartCost = pp.Product.StandartCost,
                    ProductTypeId = pp.Product.ProductTypeId,  
                    Manufacturer = pp.Product.Manufacturer,
                    Description = pp.Product.Description
                  }).OrderBy(p => p.Quantity == 0).ThenBy(p => p.Quantity)
                                .ToListAsync();

            return products;
        }


        public async Task<IEnumerable<ProductParameter>> GetSortProductParameters(long id)
        {
            var value = await _context.ProductParameters.Include(y => y.Parameter)
                                       .Include(b => b.Product)
                                       .Where(p => p.ParameterId == id)
                                       .OrderBy(t => t.Value)
                                       .ToListAsync();
            return value;
        }

        public async Task<double> GetSumSteelBars(int value)
        {
            string diametr = value.ToString();

            return await _context.ProductParameters
                .Include(p => p.Product)
                .Where(n => n.Product.ProductTypeId == 1)
                .Where(f => f.Value == value.ToString())
                .SumAsync(a => a.Product.Quantity);
        }

        public async Task<double> GetSumTubes(int value)
        {
            string diametr = value.ToString();

            return await _context.ProductParameters
                .Include(p => p.Product)
                .Where(n => n.Product.ProductTypeId == 2)
                .Where(f => f.Value == value.ToString())
                .SumAsync(a => a.Product.Quantity);
        }

        public async Task<double> GetSumQuantityByParam(int typeId, string param, int paramId, int paramDiameterId, string diameter)
        {
            var sumQuantity =  await _context.ProductParameters
                        .Include(p => p.Product)
                        .Where(prod => prod.Product.ProductTypeId == typeId)
                        .Where(pp1 => pp1.Value == param && pp1.ParameterId == paramId)
                        .Join(_context.ProductParameters.Where(pp => pp.ParameterId == paramDiameterId && pp.Value == diameter),
                                pp1 => pp1.Product.Id,
                                pp => pp.Product.Id,
                               (pp1, pp) => new ProductParameter
                               {
                                   Product = pp1.Product
                               }).SumAsync(a => a.Product.Quantity);

            return Math.Round(sumQuantity, 2);
        }

        public async Task<double> GetSumPriceByParam(int typeId, string param, int paramId, int paramDiameterId, string diameter)
        {
            var sumProductStandartCost = await _context.ProductParameters
                        .Include(p => p.Product)
                        .Where(prod => prod.Product.ProductTypeId == typeId)
                        .Where(pp1 => pp1.Value == param && pp1.ParameterId == paramId)
                        .Join(_context.ProductParameters.Where(pp => pp.ParameterId == paramDiameterId && pp.Value == diameter),
                               pp1 => pp1.Product.Id,
                               pp => pp.Product.Id,
                               (pp1, pp) => new ProductParameter
                               {
                                   Product = pp1.Product,

                               }).SumAsync(a => a.Product.Quantity * a.Product.StandartCost);

            return Math.Round(sumProductStandartCost, 2);
        }
    }
}
