using gdm5._0.Models;
using gdm5._0.Requests.PriceList;

namespace gdm5._0.Services.Interfaces
{
    public interface IPriceListService : IBaseServices<PriceList>
    {
        public void AddPriceList(addPriceListRequest addPriceList);
        public PriceListWithValues GetPriceList(string namePriceList);
        public PriceListWithValues GetPriceListWithValues(string namePriceList);
        public void UpdatePriceList(addPriceListRequest addPriceList);
        public void DeletePriceList(int priceListId);
    }
}
