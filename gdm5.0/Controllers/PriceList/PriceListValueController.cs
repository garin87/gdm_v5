using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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




    }
}