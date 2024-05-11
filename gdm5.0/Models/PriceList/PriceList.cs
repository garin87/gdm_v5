using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace gdm5._0.Models
{
    public class PriceList : BaseObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Version { get; set; }
        public string Currency { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }


        [ForeignKey("ComplexPriceListId")]
        public int? ComplexPriceListId { get; set; }
        public virtual ComplexPriceList ComplexPriceList { get; set; }
        public ICollection<PriceListValue> PriceListValue { get; set; } = new List<PriceListValue>();
    }
}
