using System;
using System.Collections.Generic;

namespace gdm5._0.Domain.Models.Filters
{
    public sealed class ProductFilter : PagingFilter
    {
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public double? Quantity { get; set; }
        public double? PrimeCost { get; set; }
        public double? StandartCost { get; set; }
        public int? ProductTypeId { get; set; }
        
        public ProductParametersFilter[] Parameters { get; set; } = Array.Empty<ProductParametersFilter>();
    }
}
