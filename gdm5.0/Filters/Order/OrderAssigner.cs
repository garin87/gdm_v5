

using gdm5._0.Domain.Models.Filters;
using gdm5._0.Models;
using System.Linq;

namespace gdm5._0.Filters
{

    public sealed class OrderAssigner : FilterAssigner<Order, OrderFilter>
    {
        public OrderAssigner(OrderFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Apply the parameter filter to the query.
        /// </summary>
        public override IQueryable<Order> ApplyFilter(IQueryable<Order> orderInstances)
        {
            if (Filter == null)
                return orderInstances;

            var orderCreatedTime = Filter.OrderCreatedTime;
            if (orderCreatedTime.HasValue)
                orderInstances = orderInstances.Where(c => c.OrderCreatedTime == orderCreatedTime);

            var filterStartDate = Filter.FilterStartDate;
            if (filterStartDate.HasValue)
                orderInstances = orderInstances.Where(c => c.OrderCreatedTime >= filterStartDate);

            var filterEndDate = Filter.FilterEndDate;
            if (filterEndDate.HasValue)
                orderInstances = orderInstances.Where(c => c.OrderCreatedTime <= filterEndDate);

            var nameCompany = Filter.NameCompany;
            if (!string.IsNullOrWhiteSpace(nameCompany))
                orderInstances = orderInstances.Where(c => c.NameCompany.Contains(nameCompany));

            //var nameProduct = Filter.NameProduct;
            //if (!string.IsNullOrWhiteSpace(nameProduct))
            //    orderInstances = orderInstances.Where(c => (c.OrderProduct as OrderProduct)
            //                                   .Product.ProductType.NameType.Contains(nameProduct));

            var orderNumber = Filter.Number;
            if (orderNumber.HasValue)
                orderInstances = orderInstances.Where(c => c.OrderNumber == orderNumber);

            var orderTotalPrice = Filter.TotalPrice;
            if (orderTotalPrice.HasValue)
                orderInstances = orderInstances.Where(c => c.TotalPrice == orderTotalPrice);

            var orderCustomerId = Filter.CustomerId;
            if (orderCustomerId.HasValue)
                orderInstances = orderInstances.Where(c => c.CustomerId == orderCustomerId);

            return orderInstances;
        }
    }
}
