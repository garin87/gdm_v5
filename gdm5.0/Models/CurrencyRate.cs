using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class CurrencyRate : BaseObject
    {
        // BLR Rate
        public double DayOfRate { get; set; }
        public DateTime DateTime { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
    }
}
