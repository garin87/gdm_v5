using System.Collections.Generic;
using gdm5._0.Models;
using gdm5._0.Requests.PriceList;

namespace gdm5._0.Services.Interfaces
{
    public interface IPriceListValueService : IBaseServices<PriceListValue>
    {
        public void AddPriceListValues(addPriceListValueRequest addPriceListValues);
        public void UpdatePriceListValues(addPriceListValueRequest addPriceListValues);
        public void UpdatePriceListValueProduct(addPriceListValueRequest addPriceListValue);
        public List<PriceListValue> GetPriceListValues(int priceListId);
        public void DeletePriceListValues(int PriceListValueProductUniqCode);
    }
}
