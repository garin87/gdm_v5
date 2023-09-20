using gdm5._0.Models;
using gdm5._0.Requests.PriceList;
using gdm5._0.Requests.Product;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace gdm5._0
{
    // [Authorize(Roles = "admin")] 
    [Route("api/pricelist")]
    [ApiController]
    public class PriceListController : Controller
    {
        private readonly DataContext _context;
        private readonly IPriceListService _priceListService;
        public PriceListController(DataContext context, IPriceListService PriceListService) 
        {
            _context = context;
            _priceListService = PriceListService;
        }

        // GET: api/pricelist/GetAll
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            
            return Ok(_priceListService.GetAll());
        }

        // GET: api/pricelist/GetPriceList/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPriceList(int id)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _priceListService.GetItem(id);

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        // POST: api/pricelist/AddPriceList
        [HttpPost]
        public async Task<IActionResult> Add(PriceList piceList)
        {    
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productNew = await _priceListService.AddItem(piceList);

            return Ok(productNew);
        }

        // DELETE: api/pricelist/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _priceListService.DeleteItem(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }


        [HttpPost]
        [Route("addPriceList")]
        public IActionResult AddPriceList(addPriceListRequest addPriceList)
        {

            if (addPriceList == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect data" });

            try
            {
                _priceListService.AddPriceList(addPriceList);
                return Ok(new { IsSuccess = true, Message = "Success: " + addPriceList.Name.Value + " has cteated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("updatePriceList")]
        [HttpPost]
        public IActionResult UpdatePriceList(addPriceListRequest addPriceList)
        {

            try
            {
                _priceListService.UpdatePriceList(addPriceList);
                return Ok(new { IsSuccess = true, Message = "Success: " + addPriceList.Name.Value + " has updated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }
      
        [Route("getPriceListWithValuesByName/{namePriceList}")]
        public IActionResult GetPriceListWithValues([FromRoute] string namePriceList)
        {
            try
            {
                PriceListWithValues priceListValues = _priceListService.GetPriceListWithValues(namePriceList);

                if (priceListValues == null)
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

        //[Route("getPriceListWithValues/{priceListId}")]
        //public IActionResult GetPriceListWithValues(int priceListId)
        //{
        //    try
        //    {
        //        PriceListWithValues priceListValues = _priceListService.GetPriceListWithValues(priceListId);

        //        if (priceListValues == null)
        //        {
        //            return NotFound("No price list values found for the specified price list ID.");
        //        }

        //        return Ok(priceListValues);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "An error occurred while processing the request.");
        //    }
        //}

        [HttpDelete]
        [Route("deletePriceList")]
        public IActionResult DeletePriceList(int priceListValueIds)
        {
            try
            {
                _priceListService.DeletePriceList(priceListValueIds);

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