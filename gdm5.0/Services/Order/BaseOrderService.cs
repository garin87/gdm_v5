using System;
using System.Linq;
using gdm5._0.Models;
using gdm5._0.Services;
using gdm5._0.Shared.Constants;

namespace gdm5._0.Services.OrderB
{
    public class BaseOrderService<T> : BaseService<T> where T : BaseObject
    {
        public BaseOrderService(DataContext context) : base(context)
        {
        }

        protected void ValidateNameCompany(string NameCompany)
        {
            if (string.IsNullOrEmpty(NameCompany))
                throw new ApplicationException("Enter valid name company");
        }


        protected int GetCustomerByName(string customerName)
        {
            if (string.IsNullOrEmpty(customerName))
                throw new ApplicationException("Entered name of company does not exist");

            var company = _context.Customer.FirstOrDefault(Customer => (Customer.NameCompany.ToLower() == customerName.ToLower()));
            var companyId = 0;
            if (company != null)
            {
                companyId = company.Id;
            }
            else throw new ApplicationException("Entered name of company does not exist");

            return companyId;
        }

        protected int GetCompletedOrderStatusId()
        {
            return _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Completed).Id;
        }

        protected int GetProcessingOrderStatusId()
        {
            return _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing).Id;
        }

    }
}


