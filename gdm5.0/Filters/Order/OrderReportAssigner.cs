

using gdm5._0.Domain.Models.Filters;
using gdm5._0.Models;
using System.Linq;

namespace gdm5._0.Filters
{

    public sealed class OrderReportAssigner : FilterAssigner<Order, OrderReportFilter>
    {
        public OrderReportAssigner(OrderReportFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Apply the parameter filter to the query.
        /// </summary>
        public override IQueryable<Order> ApplyFilter(IQueryable<Order> orderInstances)
        {
            if (Filter == null)
                return orderInstances;

            var filterStartDate = Filter.FilterStartDate;
            if (filterStartDate.HasValue)
                orderInstances = orderInstances.Where(c => c.OrderCreatedTime >= filterStartDate);

            var filterEndDate = Filter.FilterEndDate;
            if (filterEndDate.HasValue)
                orderInstances = orderInstances.Where(c => c.OrderCreatedTime <= filterEndDate);

            var nameCompany = Filter.NameCompany;
            if (!string.IsNullOrWhiteSpace(nameCompany))
                orderInstances = orderInstances.Where(c => c.NameCompany.Contains(nameCompany));

            return orderInstances;
        }
    }
}
