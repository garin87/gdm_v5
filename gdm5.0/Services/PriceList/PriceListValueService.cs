
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;


namespace gdm5._0.Services
{
    public class PriceListValueService : BaseService<PriceListValue>, IPriceListValueService
    {
        private readonly DataContext _context;

        public PriceListValueService(DataContext context) : base(context)
        {
            _context = context;
        }

    }


}

