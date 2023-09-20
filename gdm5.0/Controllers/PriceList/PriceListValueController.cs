using gdm5._0.Models;
using gdm5._0.Requests.PriceList;
using gdm5._0.Requests.Product;
using gdm5._0.Services.Interfaces;
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
        private readonly DataContext _context;
        private readonly IPriceListValueService _priceListValueService;
        public PriceListValueController(DataContext context, IPriceListValueService PriceListValueService) 
        {
            _context = context;
            _priceListValueService = PriceListValueService;
        }

        // GET: api/pricelistvalue/GetAll
        [HttpGet]
        public IActionResult GetAll()
        {
            
            return Ok(_priceListValueService.GetAll());
        }

        // GET: api/pricelistvalue/GetProduct/5
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

        [HttpPost]
        [Route("addPriceListValues")]
        public IActionResult AddPriceList(addPriceListValuesRequest addPriceListValues)
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

        [HttpPost]
        [Route("updatePriceListValues")]
        public IActionResult UpdatePriceList(addPriceListValuesRequest addPriceListValues)
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
        }


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

        [HttpDelete]
        [Route("deletePriceListValues")]
        public IActionResult DeletePriceListValues(List<int> priceListValueIds)
        {
            try
            {
                _priceListValueService.DeletePriceListValues(priceListValueIds);

                return NoContent();
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