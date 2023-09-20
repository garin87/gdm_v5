using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.PriceList
{
    public class addPriceListValueRequest
    {
        public int? idPriceListValue { get; set; }
        public Property Description { get; set; }
        public Property Quantity { get; set; }
        public Property Price { get; set; }
        public Property Version { get; set; }
    
        public int ProductId { get; set; }
        public int PriceListId { get; set; }
    }
}
