

using gdm5._0.Domain.Models.Filters;
using gdm5._0.Models;
using gdm5._0.Shared;
using System;
using System.Linq;

namespace gdm5._0.Filters
{

    public sealed class ProductHistoryAssigner : FilterAssigner<ProductHistory, ProductFilter>
    {
        public ProductHistoryAssigner(ProductFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Apply the parameter filter to the query.
        /// </summary>
        public override IQueryable<ProductHistory> ApplyFilter(IQueryable<ProductHistory> productInstances)
        {
            if (Filter == null)
                return productInstances;

            //var productTypeId = Filter.ProductTypeId;
            //if (productTypeId.HasValue)
            //    productInstances = productInstances.Where(c => c.ProductTypeId == productTypeId);

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
                        c.ProductParameterHistory.Any(pp => pp.Value.Equals(parameter.Value)));
                    }

                }

            }

            //GlobalVariables.TotalRecords = productInstances.Count();
            //GlobalVariables.TotalQuantity = productInstances.Sum(product => product.Quantity);
            //GlobalVariables.TotalPrimeCost = productInstances.Sum(product => product.PrimeCost * product.Quantity);
            //GlobalVariables.TotalPrimeCostEUR = productInstances.Sum(product => product.PrimeCostEUR * product.Quantity);
            //GlobalVariables.TotalPrimeCostUSD = productInstances.Sum(product => product.PrimeCostUSD * product.Quantity);

            return productInstances;
        }
    }
}
