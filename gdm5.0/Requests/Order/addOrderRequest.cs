using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.Product
{
    public class AddOrderRequest
    {
        public int ProductId { get; set; }
        public bool isAddToCart { get; set; }
        public Property company { get; set; }
        public Property currency { get; set; }
        public Property quantityorder { get; set; }
        public Property totalprice { get; set; }
        public Property taxnds { get; set; }
        public Property markup { get; set; }
        public Property primecost { get; set; }
        public Property placeofstoragedetail { get; set; }
        public Property description { get; set; }
        public Property currencyname { get; set; }
        public Property warehouse { get; set; }
        public Property number { get; set; }
    }
}
