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
    [Route("api/warehouse")]
    [ApiController]
    public class WareHousesController : Controller
    {
        private readonly DataContext _context;
        private readonly IWareHouseService _wareHouseService;
        public WareHousesController(DataContext context, IWareHouseService WareHouseService) 
        {
            _context = context;
            _wareHouseService = WareHouseService;
        }


        // GET: api/warehouse/GetAll
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            
            return Ok(_wareHouseService.GetAll());
        }

        // GET: api/warehouse/GetWareHouse/5
        [HttpGet("{id}")]
        [Route("GetWareHouse")]
        public async Task<IActionResult> GetWareHouse(int id)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _wareHouseService.GetItem(id);

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        // POST: api/warehouse/AddWareHouse
        [HttpPost]
        public async Task<IActionResult> AddWareHouse(WareHouse wareHouse)
        {    
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productNew = await _wareHouseService.AddItem(wareHouse);

            return Ok(productNew);
        }

        [Route("DeleteWareHouse/{id}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWareHouse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _wareHouseService.DeleteItem(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [Route("GetWareHouseByName/{nameWareHouse}")]
        [HttpGet]
        public IActionResult GetCustomerByNameCompany([FromRoute] string nameWareHouse)
        {
            try
            {
                var result = this._wareHouseService.GetWareHouseByName(nameWareHouse);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("GetNamesWareHouses")]
        [HttpGet]
        public IActionResult GetNamesWareHouses()
        {
            try
            {
                var result = this._wareHouseService.GetNamesWareHouses();
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("AddNewWareHouse")]
        public async Task<IActionResult> AddNewWareHouse(addWareHouseRequest newWareHouse)
        {

            if (newWareHouse == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = await _wareHouseService.AddNewWareHouse(newWareHouse);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name + " has cteated."});
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("UpdateWareHouse")]
        public async Task<IActionResult> UpdateWareHouse(UpdateWareHouseRequest warehouse)
        {

            if (warehouse == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = await _wareHouseService.UpdateWareHouse(warehouse);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name + " has cteated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

    }
}