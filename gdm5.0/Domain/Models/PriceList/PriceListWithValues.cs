using System.Collections.Generic;
using gdm5._0.Models;

namespace gdm5._0.Requests.PriceList
{
    public class PriceListWithValues
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Version { get; set; }
        public string CreationDate { get; set; }
        public string LastModifiedDate { get; set; }
        public ICollection<PriceListValueDTO> PriceListValue { get; set; } = new List<PriceListValueDTO>();
    }
}
