

using gdm5._0.Domain.Models.Filters;
using gdm5._0.Models;
using System;
using System.Linq;

namespace gdm5._0.Filters
{

    public sealed class ProductParametersAssigner : FilterAssigner<Product, ProductParametersFilter>
    {
        public ProductParametersAssigner(ProductParametersFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Apply the parameter filter to the query.
        /// </summary>
        public override IQueryable<Product> ApplyFilter(IQueryable<Product> productParameteres)
        {
            if (Filter == null)
                return productParameteres;


            var productParamValue = Filter.Value;
            if (!string.IsNullOrWhiteSpace(productParamValue))
            {
                productParameteres = productParameteres.Where(c =>
                c.ProductParameters.Any(pp => pp.Value.Equals(productParamValue)));
            }
               

            //var paramName = Filter.ParameterName;
            //if (!string.IsNullOrWhiteSpace(paramName))
            //    productParameteres = productParameteres.Where(c => c.Parameter.Name.Contains(paramName));

            //var parameterId = Filter.ParameterId;
            //if (parameterId.HasValue)
            //    productParameteres = productParameteres.Where(c => c.ParameterId == parameterId.Value);

            //var productId = Filter.ProductId;
            //if (productId.HasValue)
            //    productParameteres = productParameteres.Where(c => c.ProductId == productId.Value);

            return productParameteres;
        }
    }
}
