using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Requests.Customer;
using gdm5._0.Requests.Product;
using gdm5._0.Services;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace gdm5._0
{
    // [Authorize(Roles = "admin")] 
    [Route("api/customer")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly DataContext _context;
        private readonly ICustomerService _customerService;
        public CustomersController(DataContext context, ICustomerService CustomerService) 
        {
            _context = context;
            _customerService = CustomerService;
        }


        // GET: api/customer/GetAll
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            
            return Ok(_customerService.GetAll());
        }

        // GET: api/customer/GetProduct/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _customerService.GetItem(id);

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        // POST: api/customer/AddCustomer
        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {    
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productNew = await _customerService.AddItem(customer);

            return Ok(productNew);
        }

        [Route("DeleteCustomer/{id}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _customerService.DeleteItem(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(new { IsSuccess = false, Message = " The company deleted" });
        }


        [Route("getCustomerByNameCompany/{nameCompany}")]
        [HttpGet]
        public IActionResult GetCustomerByNameCompany([FromRoute] string nameCompany)
        {
            try
            {
                var result = this._customerService.GetCustomerByNameCompany(nameCompany);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("GetNamesCustomeres")]
        [HttpGet]
        public IActionResult GetNamesCustomeres()
        {
            try
            {
                var result = this._customerService.GetNamesCompanies();
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("AddNewCustomer")]
        public async Task<IActionResult> AddNewCustomer(addCustomerRequest newCustomer)
        {

            if (newCustomer == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = await _customerService.AddNewCustomer(newCustomer);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.NameCompany + " has cteated."});
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerRequest customer)
        {

            if (customer == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = await _customerService.UpdateCustomer(customer);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.NameCompany + " has cteated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

    }
}