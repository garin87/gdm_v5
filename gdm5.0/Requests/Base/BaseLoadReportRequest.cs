using System;
using gdm5._0.Domain.Models.Filters;
using gdm5._0.DTO;

namespace gdm5._0.Requests.Base
{
    public class BaseLoadReportRequest
    {
        public Property Name { get; set; }
        public Property Manufacturer { get; set; }
        public Property Supplier { get; set; }
        public ProductParametersFilter[] Parameters { get; set; } = Array.Empty<ProductParametersFilter>();
    }
}
