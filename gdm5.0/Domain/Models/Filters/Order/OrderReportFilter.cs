using System;
using System.Collections.Generic;

namespace gdm5._0.Domain.Models.Filters
{
    public sealed class OrderReportFilter : PagingFilter
    {
        public string NameCompany { get; set; }
        public string NameProduct { get; set; }
        public string Manufacturer { get; set; }
        public string Supplier { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }

        public ProductParametersFilter[] Parameters { get; set; } = Array.Empty<ProductParametersFilter>();
    }
}
