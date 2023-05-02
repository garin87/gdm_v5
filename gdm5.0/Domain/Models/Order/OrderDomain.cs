using System;
using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Domain.Models.Order
{
    public class OrderDomain
    {
        public string NameCompany { get; set; }
        public double? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public double? TaxNDS { get; set; }
        public double? Markup { get; set; }
        public int? Number { get; set; }
        public string OrderCreatedTime { get; set; }
        public string OrderCreatedByUser { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }

        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        public string Manufacturer { get; set; }

        public ICollection<ParameterDTO> Parameters { get; set; } = new List<ParameterDTO>();
    }
}
