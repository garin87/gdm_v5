using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace gdm5._0
{
    // [Authorize(Roles = "admin")] 
    [Route("api/complexpricelist")]
    [ApiController]
    public class ComplexPriceListController : Controller
    {
        private readonly IComplexPriceListService _complexPriceListService;
        public ComplexPriceListController(IComplexPriceListService ComplexPriceListService) 
        {
            _complexPriceListService = ComplexPriceListService;
        }

        // GET: api/complexpricelist/GetAll
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            
            return Ok(_complexPriceListService.GetAll());
        }

        // GET: api/complexpricelist/GetComplexPriceList/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComplexPriceList(int id)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _complexPriceListService.GetItem(id);

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        // POST: api/complexpricelist/AddComplexPriceList
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComplexPriceList(ComplexPriceList piceList)
        {    
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productNew = await _complexPriceListService.AddItem(piceList);

            return Ok(productNew);
        }

        // DELETE: api/complexpricelist/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplexPriceList([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _complexPriceListService.DeleteItem(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }




    }
}