

using gdm5._0.Domain.Models.Filters;
using gdm5._0.Models;
using gdm5._0.Shared;
using System;
using System.Linq;

namespace gdm5._0.Filters
{

    public sealed class ProductAssigner : FilterAssigner<Product, ProductFilter>
    {
        public ProductAssigner(ProductFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Apply the parameter filter to the query.
        /// </summary>
        public override IQueryable<Product> ApplyFilter(IQueryable<Product> productInstances)
        {
            if (Filter == null)
                return productInstances;

            var productTypeId = Filter.ProductTypeId;
            if (productTypeId.HasValue)
                productInstances = productInstances.Where(c => c.ProductTypeId == productTypeId);

            var productName = Filter.Name;
            if (!string.IsNullOrWhiteSpace(productName))
                productInstances = productInstances.Where(c => c.Name.Equals(productName));

            var productNumber = Filter.ProductNumber;
            if (!string.IsNullOrWhiteSpace(productNumber))
                productInstances = productInstances.Where(c => c.ProductNumber.Equals(productNumber));

            var productManufacturer = Filter.Manufacturer;
            if (!string.IsNullOrWhiteSpace(productManufacturer))
                productInstances = productInstances.Where(c => c.Manufacturer.Equals(productManufacturer));

            var productQuantity = Filter.Quantity;
            if (productQuantity.HasValue)
                productInstances = productInstances.Where(c => c.Quantity == productQuantity);

            var productPrimeCost = Filter.PrimeCost;
            if (productPrimeCost.HasValue)
                productInstances = productInstances.Where(c => c.PrimeCost == productPrimeCost);

            var productStandartCost = Filter.Quantity;
            if (productStandartCost.HasValue)
                productInstances = productInstances.Where(c => c.StandartCost == productStandartCost);

            var productParameters = Filter.Parameters;
            if (productParameters.Length > 0)
            {
                foreach (var parameter in productParameters)
                {
                    if (!string.IsNullOrWhiteSpace(parameter.Value))
                    {
                        productInstances = productInstances.Where(c =>
                        c.ProductParameters.Any(pp => pp.Value.Equals(parameter.Value) &&
                                                pp.ParameterId == parameter.ParameterId));
                    }

                }

            }

            calculateTotal(productInstances);

            return productInstances;
        }

        private void calculateTotal(IQueryable<Product> productInstances)
        {
            var totals = productInstances
            .GroupBy(p => 1)
            .Select(group => new {
                 TotalRecords = group.Count(),
                 TotalQuantity = group.Sum(p => p.Quantity),
                 TotalPrimeCost = group.Sum(p => p.PrimeCost * p.Quantity),
                 TotalPrimeCostEUR = group.Sum(p => p.PrimeCostEUR * p.Quantity),
                 TotalPrimeCostUSD = group.Sum(p => p.PrimeCostUSD * p.Quantity)
             })
            .FirstOrDefault();

            if (totals != null)
            {
                GlobalVariables.TotalRecords = totals.TotalRecords;
                GlobalVariables.TotalQuantity = totals.TotalQuantity;
                GlobalVariables.TotalPrimeCost = totals.TotalPrimeCost;
                GlobalVariables.TotalPrimeCostEUR = totals.TotalPrimeCostEUR;
                GlobalVariables.TotalPrimeCostUSD = totals.TotalPrimeCostUSD;
            }
            else
            {
                GlobalVariables.TotalRecords = 0;
                GlobalVariables.TotalQuantity = 0;
                GlobalVariables.TotalPrimeCost = 0;
                GlobalVariables.TotalPrimeCostEUR = 0;
                GlobalVariables.TotalPrimeCostUSD = 0;
            }

        }
    }
}
