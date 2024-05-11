using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using gdm5._0.Services.OrderB;

namespace gdm5._0.Services
{
    public class OrderProductService : BaseOrderService<OrderProduct>, IOrderProductService
    {

        public OrderProductService(DataContext context) : base(context)
        {
        }
   
        public async Task<OrderProduct> UpdateOrderProduct(int id, OrderProduct orderProduct)
        {
            OrderProduct p = await GetItem(orderProduct.Id);
            p.Quantity = orderProduct.Quantity;
            p.Order = orderProduct.Order;
            p.Product = orderProduct.Product;

            await _context.SaveChangesAsync();

            return orderProduct;
        }

    }
}
