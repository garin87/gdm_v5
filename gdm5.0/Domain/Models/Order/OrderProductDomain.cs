using System;
using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Domain.Models.Order
{
    public class OrderProductDomain
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        public string Manufacturer { get; set; }
        public double Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double TaxNDS { get; set; }
        public double Markup { get; set; }
        public ICollection<ParameterDTO> Parameters { get; set; } = new List<ParameterDTO>();
    }
}
