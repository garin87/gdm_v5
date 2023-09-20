using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class PagedResponseDTO<T> : ResponseDTO<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Uri FirstPage { get; set; }
        public Uri LastPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalPrimeCost { get; set; }
        public double TotalPrimeCostEUR { get; set; }
        public double TotalPrimeCostUSD { get; set; }

        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }
        public PagedResponseDTO(T data, int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
