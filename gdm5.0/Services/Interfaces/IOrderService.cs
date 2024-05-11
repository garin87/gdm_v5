using gdm5._0.Domain.Models.Order;
using gdm5._0.Models;
using gdm5._0.Requests.Order;
using gdm5._0.Requests.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
    public interface IOrderService : IBaseServices<Order>
    {
        string[] getOrderNameCompanies();
        string[] getOrderNameCompanies(string NameCompany);
        string[] getNamesProduct();
        Task addOrderProductList(addOrderListProductRequest orderData, int currentUserId, string currentUserName);
        List<OrderTotalQuantity> LoadOrderReport(loadOrderReportRequest request);
    }
}
