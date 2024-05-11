using gdm5._0.Models;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
      interface IOrderProductService : IBaseServices<OrderProduct>
      {
        Task<OrderProduct> UpdateOrderProduct(int id, OrderProduct orderProduct);
      }
}
