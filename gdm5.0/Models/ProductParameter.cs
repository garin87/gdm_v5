using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace gdm5._0.Models
{
    public class ProductParameter : BaseObject
    {
        // public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int ParameterId { get; set; }
        public virtual Parameter Parameter { get; set; }

        public string Value { get; set; }

    }

}
