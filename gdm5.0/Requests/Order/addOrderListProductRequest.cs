using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.Product
{
    public class addOrderListProductRequest
    {

        public Property company { get; set; }
        public Property currency { get; set; }
        public Property currencyname { get; set; }
        public Property ordernumber { get; set; }
        public Property description { get; set; }

        public ICollection<OrderProductRequest> orderProductList { get; set; } = new List<OrderProductRequest>();
        
    }
}
