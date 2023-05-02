

using gdm5._0.Domain.Models.Filters;
using gdm5._0.Models;
using System.Linq;

namespace gdm5._0.Filters
{

    public sealed class ParametersAssigner : FilterAssigner<Parameter, ParametersFilter>
    {
        public ParametersAssigner(ParametersFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Apply the parameter filter to the query (paging ignored).
        /// </summary>
        public override IQueryable<Parameter> ApplyFilter(IQueryable<Parameter> parameteres)
        {
            if (Filter == null)
                return parameteres;

            var productTypeId = Filter.ProductTypeId;
            if (productTypeId.HasValue)
                parameteres = parameteres.Where(h => h.ProductTypeId == productTypeId.Value);

            var paramName = Filter.Name;
            if (!string.IsNullOrWhiteSpace(paramName))
                parameteres = parameteres.Where(c => c.Name.Contains(paramName));

            //var paramNameType = Filter.NameType;
            //if (!string.IsNullOrWhiteSpace(paramNameType))
            //    parameteres = parameteres.Where(c => c.NameType.Contains(paramNameType));
            
            //var navigationOrder = Filter.NavigationOrder;
            //if (navigationOrder.HasValue)
            //    parameteres = parameteres.Where(h => h.NavigationOrder == navigationOrder.Value);

            return parameteres;
        }
    }
}
