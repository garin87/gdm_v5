using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddPriceList(PriceList piceList)
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
        public async Task<IActionResult> DeletePriceList([FromRoute] int id)
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




    }
}