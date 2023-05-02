using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Domain.Models.Filters;
using gdm5._0.DTO;

namespace gdm5._0.Requests.Order
{
    public sealed class getOrdersRequest
    {
        public string Name { get; set; }
        public int OrderId { get; set; }
        public SortOptionsDTO SortOption { get; set; }
        public PaginationFilterDTO PageFilter { get; set; }
        public OrderFilter Filter { get; set; }

    }
}
