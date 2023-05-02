using System;
using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using gdm5._0.Requests.Customer;
using System.Linq;

namespace gdm5._0.Services
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly DataContext _context;

        public CustomerService(DataContext context) : base(context)
        {
            _context = context;
        }

        public Customer GetCustomerByNameCompany(string nameCompany)
        {

            if (string.IsNullOrEmpty(nameCompany))
                throw new ApplicationException("Enter valid name company");


            var existcompany = _context.Customer.FirstOrDefault(customer => customer.NameCompany.ToLower() == nameCompany.ToLower());
            if (existcompany == null) throw new ApplicationException("Entered name of company does not exist");

            return existcompany;
        }

        public string[] GetNamesCompanies()
        {
            return _context.Customer.Select(customer => customer.NameCompany).ToArray();
        }

        public async Task<Customer> AddNewCustomer(addCustomerRequest newCustomer)
        {

            if (string.IsNullOrEmpty(newCustomer.NameCompany?.Value))
                throw new ApplicationException("Enter valid name company");


            var existcompany = _context.Customer.FirstOrDefault(customer => customer.NameCompany == newCustomer.NameCompany.Value);
            if (existcompany != null) throw new ApplicationException("Entered name of company exists");


            var customer = new Customer()
            {
                NameCompany = newCustomer.NameCompany.Value,
                AddressCompany = newCustomer.AddressCompany?.Value ?? "",
                City = newCustomer.City?.Value ?? "",
                CustomerName = newCustomer.CustomerName?.Value ?? "",
                CustomerNameSecond = newCustomer.CustomerName2?.Value ?? "",
                MobilePhone = newCustomer.MobilePhone?.Value ?? "",
                MobilePhoneSecond = newCustomer.MobilePhone2?.Value ?? "",
                Description = newCustomer.Description?.Value ?? "",
                Priority = newCustomer.Priority?.Value ?? "",
                PriorityColor = newCustomer.PriorityColor?.Value ?? ""

            };

            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();


            return customer;
        }

        public async Task<Customer> UpdateCustomer(UpdateCustomerRequest updatedCustomer)
        {
            if (updatedCustomer.CustomerId == 0)
                throw new ApplicationException("Enter valid CustomerId");

            if (string.IsNullOrEmpty(updatedCustomer.NameCompany?.Value))
                throw new ApplicationException("Enter valid name company");


            var existcompany = _context.Customer.FirstOrDefault(customer => customer.Id == updatedCustomer.CustomerId);
            if (existcompany == null) throw new ApplicationException("Entered customer does not exist");


            existcompany.NameCompany = updatedCustomer.NameCompany.Value; // needs check existed name 
            existcompany.AddressCompany = updatedCustomer.AddressCompany?.Value ?? "";
            existcompany.City = updatedCustomer.City?.Value ?? "";
            existcompany.CustomerName = updatedCustomer.CustomerName?.Value ?? "";
            existcompany.CustomerNameSecond = updatedCustomer.CustomerName2?.Value ?? "";
            existcompany.MobilePhone = updatedCustomer.MobilePhone?.Value ?? "";
            existcompany.MobilePhoneSecond = updatedCustomer.MobilePhone2?.Value ?? "";
            existcompany.Description = updatedCustomer.Description?.Value ?? "";
            existcompany.Priority = updatedCustomer.Priority?.Value ?? "";
            existcompany.PriorityColor = updatedCustomer.PriorityColor?.Value ?? "";

            await _context.SaveChangesAsync();
            return existcompany;
        }

    }


}

