using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.PriceList
{
    public class addPriceListValuesRequest
    {
        public ICollection<addPriceListValueRequest> PriceListValues { get; set; } = new List<addPriceListValueRequest>();
    }
}
