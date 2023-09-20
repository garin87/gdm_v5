using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.PriceList
{
    public class addPriceListRequest 
    {
        public int? idPriceList { get; set; }
        public Property Name { get; set; }
        public Property Description { get; set; }
        public Property Version { get; set; }
    }
}
