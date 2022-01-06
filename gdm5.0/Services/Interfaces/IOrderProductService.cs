using gdm5._0.DTO;
using gdm5._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
      interface IOrderProductService : IBaseServices<OrderProduct>
    {
        Task<OrderProduct> UpdateOrderProduct(int id, OrderProduct orderProduct);
        Task<OrderProduct> UpdateOrderP(OrderDTO orderDTO);
        Task<IEnumerable<OrderPDTO>> GetOrderByPruductId(int id);
    }
}
