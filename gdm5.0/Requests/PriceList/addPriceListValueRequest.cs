using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.PriceList
{
    public class addPriceListValueRequest
    {

        public Property Description { get; set; }
        public Property Quantity { get; set; }
        public Property PercentOfMarkup { get; set; }
        public Property Price { get; set; }
        public Property PriceNDS { get; set; }
        public Property Version { get; set; }
        public Property Unit { get; set; }
        public Property FilterStartDimension { get; set; }
        public Property FilterEndDimension { get; set; }
        public Property Name { get; set; }
        public Property Manufacturer { get; set; }
        public Property Supplier { get; set; }
        public ICollection<Property> Parameters { get; set; } = new List<Property>();

        public int PriceListValueProductUniqCode { get; set; }
        public int? idPriceListValue { get; set; }
        public int ProductId { get; set; }
        public string PriceListName { get; set; }
        public int PriceListId { get; set; }
    }
}
