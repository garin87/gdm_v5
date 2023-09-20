using System.Collections.Generic;
using gdm5._0.Models;
using gdm5._0.Requests.PriceList;

namespace gdm5._0.Services.Interfaces
{
    public interface IPriceListValueService : IBaseServices<PriceListValue>
    {
        public void AddPriceListValues(addPriceListValuesRequest addPriceListValues);
        public void UpdatePriceListValues(addPriceListValuesRequest addPriceListValues);
        public List<PriceListValue> GetPriceListValues(int priceListId);
        public void DeletePriceListValues(List<int> priceListValueIds);
    }
}
