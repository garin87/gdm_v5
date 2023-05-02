using System;
using System.Collections.Generic;

namespace gdm5._0.Domain.Models.Filters
{
    public sealed class OrderFilter : PagingFilter
    {
        public string NameCompany { get; set; }
        public string NameProduct { get; set; }
        public double? TotalPrice { get; set; }
        public int? Number { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? OrderCreatedTime { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }

    }
}
