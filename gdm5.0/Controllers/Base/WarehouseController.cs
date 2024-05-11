using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gdm5._0.Controllers.Base
{
    public abstract class WarehouseController : Controller
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public WarehouseController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
