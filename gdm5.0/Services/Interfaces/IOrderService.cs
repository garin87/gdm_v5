using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Requests.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
    public interface IOrderService : IBaseServices<Order>
    {
        string[] getOrderNameCompanies();
        string[] getOrderNameCompanies(string NameCompany);
        string[] getNamesProduct();
        Task addOrderProductList(addOrderListProductRequest orderData);
        Task<Order> DeleteOrder(int id);
        Task<Order> UpdateOrder(int id, Order order);
        Task<OrderDTO> AddOrders(OrderDTO orderDTO);
    }
}
