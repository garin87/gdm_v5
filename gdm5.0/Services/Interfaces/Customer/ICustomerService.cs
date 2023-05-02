using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Requests.Customer;
using gdm5._0.Requests.Product;

namespace gdm5._0.Services.Interfaces
{
    public interface ICustomerService : IBaseServices<Customer>
    {
        Customer GetCustomerByNameCompany(string nameCompany);
        Task<Customer> AddNewCustomer(addCustomerRequest newCustomer);
        Task<Customer> UpdateCustomer(UpdateCustomerRequest updatedCustomer);
        string[] GetNamesCompanies();
    }
}
