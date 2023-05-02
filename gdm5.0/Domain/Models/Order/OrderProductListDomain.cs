using System;
using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Domain.Models.Order
{
    public class OrderProductListDomain
    {
        
        public int OrderId { get; set; }
        public string NameCompany { get; set; }
        public double TotalPrice { get; set; }
        public int OrderNumber { get; set; }
        public string OrderCreatedTime { get; set; }
        public string OrderCreatedByUser { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }

        public ICollection<OrderProductDomain> Products { get; set; } = new List<OrderProductDomain>();
    }
}
