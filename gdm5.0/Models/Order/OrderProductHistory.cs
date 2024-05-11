using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace gdm5._0.Models
{
    public class OrderProductHistory : BaseObject
    {

        [ForeignKey("ProductHistoryId")]
        public int ProductHistoryId { get; set; }
        public virtual ProductHistory ProductHistory { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public double Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double TaxNDS { get; set; }
        public double Markup { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
