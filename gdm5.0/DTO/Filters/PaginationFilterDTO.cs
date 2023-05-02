using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class PaginationFilterDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationFilterDTO()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PaginationFilterDTO(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 50 ? 50 : pageSize;
        }
    }
}
