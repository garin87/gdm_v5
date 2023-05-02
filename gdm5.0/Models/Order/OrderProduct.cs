using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace gdm5._0.Models
{
    public class OrderProduct : BaseObject
    {
       // public int Id { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public double Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double TaxNDS { get; set; }
        public double Markup { get; set; }
    }
}
