using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Requests.Customer;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace gdm5._0
{
    // [Authorize(Roles = "admin")] 
    [Route("api/warehouse")]
    [ApiController]
    public class WareHousesController : Controller
    {
        private readonly IWareHouseService _wareHouseService;
        public WareHousesController(IWareHouseService WareHouseService) 
        {
            _wareHouseService = WareHouseService;
        }

        // GET: api/warehouse/GetAll
        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            
            return Ok(_wareHouseService.GetAll());
        }

        // GET: api/warehouse/GetWareHouse/5
        [Authorize]
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
        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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