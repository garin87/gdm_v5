using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.Product
{
    public class OrderProductRequest
    {

        public int ProductId { get; set; }
      
        public Property quantityorder { get; set; }
        public Property totalprice { get; set; }
        public Property taxnds { get; set; }
        public Property markup { get; set; }
        public Property primecost { get; set; }

    }
}
