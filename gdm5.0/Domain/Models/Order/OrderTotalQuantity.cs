using gdm5._0.Domain.Models.Base;

namespace gdm5._0.Domain.Models.Order
{
    public class OrderTotalQuantity : BaseTotalQuantity
    {
        public double RealAmountProduct { get; set; } = 0;
    }
}
