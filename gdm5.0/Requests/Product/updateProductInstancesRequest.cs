using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.Product
{
    public class updateProductInstancesRequest
    {
        public int ProductId { get; set; }
        public Property name { get; set; }
        public Property productNumber { get; set; }
        public Property quantity { get; set; }
        public Property standartCost { get; set; }
        public Property primeCost { get; set; }
        public Property PrimeCostEUR { get; set; }
        public Property PrimeCostUSD { get; set; }
        public Property manufacturer { get; set; }
        public Property description { get; set; }
        public Property dateOfReceipt { get; set; }
        public Property currency { get; set; }
        public Property warehouse { get; set; }
        public ICollection<Property> parameters { get; set; } = new List<Property>();
    }
}
