using gdm5._0.Models;
using gdm5._0.Requests.PriceList;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gdm5._0
{
    // [Authorize(Roles = "admin")] 
    [Route("api/pricelistvalue")]
    [ApiController]
    public class PriceListValueController : Controller
    {
        private readonly IPriceListValueService _priceListValueService;
        public PriceListValueController(IPriceListValueService PriceListValueService) 
        {
            _priceListValueService = PriceListValueService;
        }

        // GET: api/pricelistvalue/GetAll
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            
            return Ok(_priceListValueService.GetAll());
        }

        // GET: api/pricelistvalue/GetProduct/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPriceListValue(int id)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _priceListValueService.GetItem(id);

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        // POST: api/pricelistvalue/AddCustomer
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPriceListValue(PriceListValue piceListValue)
        {    
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productNew = await _priceListValueService.AddItem(piceListValue);

            return Ok(productNew);
        }

        // DELETE: api/pricelistvalue/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriceListValue([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _priceListValueService.DeleteItem(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        [Route("addPriceListValues")]
        public IActionResult AddPriceList(addPriceListValueRequest addPriceListValues)
        {

            if (addPriceListValues == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect data" });

            try
            {
                _priceListValueService.AddPriceListValues(addPriceListValues);
                return Ok(new { IsSuccess = true, Message = "Success: PriceListValues has added." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("updatePriceListValues")]
        public IActionResult UpdatePriceList(addPriceListValueRequest addPriceListValues)
        {

            if (addPriceListValues == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect data" });

            try
            {
                _priceListValueService.UpdatePriceListValues(addPriceListValues);
                return Ok(new { IsSuccess = true, Message = "Success: PriceListValues has updated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getPriceListValues/{priceListId}")]
        public IActionResult GetPriceListValues(int priceListId)
        {
            try
            {
                List<PriceListValue> priceListValues = _priceListValueService.GetPriceListValues(priceListId);

                if (priceListValues == null || priceListValues.Count == 0)
                {
                    return NotFound("No price list values found for the specified price list ID.");
                }

                return Ok(priceListValues);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("deletePriceListValues/{priceListValueProductUniqCode}")]
        public IActionResult DeletePriceListValues(int priceListValueProductUniqCode)
        {
            try
            {
                _priceListValueService.DeletePriceListValues(priceListValueProductUniqCode);

                return Ok(new { IsSuccess = false, Message = "Price list value deleted" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}