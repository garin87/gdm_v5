using gdm5._0.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilterDTO filter, string route);
    }
}
