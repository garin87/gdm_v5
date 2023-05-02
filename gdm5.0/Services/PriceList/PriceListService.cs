
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;


namespace gdm5._0.Services
{
    public class PriceListService : BaseService<PriceList>, IPriceListService
    {
        private readonly DataContext _context;

        public PriceListService(DataContext context) : base(context)
        {
            _context = context;
        }
    }

}

