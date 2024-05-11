
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;


namespace gdm5._0.Services
{
    public class ComplexPriceListService : BaseService<ComplexPriceList>, IComplexPriceListService
    {

        public ComplexPriceListService(DataContext context) : base(context)
        {

        }

    }


}

