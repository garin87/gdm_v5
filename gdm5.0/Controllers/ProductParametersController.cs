using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.Services;
using Microsoft.AspNetCore.Authorization;

namespace gdm5._0
{
    // [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductParametersController : ControllerBase
    {
        private readonly DataContext _context;
        private ProductParameterService _productParameterService;

        public ProductParametersController(DataContext context)
        {
            _context = context;
            _productParameterService = new _ProductParameterService(_context);
        }


        // GET: api/ProductParameters
        [Authorize]
        [HttpGet]
        public IEnumerable<ProductParameter> GetProductParameters()
        {
            return _context.ProductParameters;
        }

        // GET: api/ProductParameters/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductParameter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productParameter = await _context.ProductParameters.FindAsync(id);

            if (productParameter == null)
            {
                return NotFound();
            }

            return Ok(productParameter);
        }

        // PUT: api/ProductParameters/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductParameter([FromRoute] int id, [FromBody] ProductParameter productParameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productParameter.Id)
            {
                return BadRequest();
            }

            _context.Entry(productParameter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductParameterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductParameters
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostProductParameter([FromBody] ProductParameter productParameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductParameters.Add(productParameter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductParameter", new { id = productParameter.Id }, productParameter);
        }

        // DELETE: api/ProductParameters/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductParameter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productParameter = await _context.ProductParameters.FindAsync(id);
            if (productParameter == null)
            {
                return NotFound();
            }

            _context.ProductParameters.Remove(productParameter);
            await _context.SaveChangesAsync();

            return Ok(productParameter);
        }

        private bool ProductParameterExists(int id)
        {
            return _context.ProductParameters.Any(e => e.Id == id);
        }

    }

    internal class _ProductParameterService : ProductParameterService
    {
        public _ProductParameterService(DataContext context) : base(context)
        {
        }
    }
}