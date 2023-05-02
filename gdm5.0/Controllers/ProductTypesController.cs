using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using gdm5._0.DTO;
using gdm5._0.Requests.Product;

namespace gdm5._0.Controllers
{
    [Route("api/productTypes")]
    [ApiController]
    public class ProductTypesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IProductTypeService _productTypeService;
        public ProductTypesController(DataContext context, IProductTypeService ProductTypeService)
        {
            _context = context;
            _productTypeService = ProductTypeService;
        }

        // GET: api/ProductTypes
        [HttpGet]
        [Route("getProductTypes")]
        public IEnumerable<ProductType> GetProductTypes()
        {
            // type.Id = 1 штока хромированные type.Id = 2 трубы хонингованные
            return _context.ProductTypes; //Take(2);
        }
        // GET: api/getProductTypeNames
        [HttpGet]
        [Route("getProductTypeNames")]
        public IEnumerable<string> GetProductTypeNames()
        {
            return _context.ProductTypes.Select(el => el.NameType);
        }

        [Route("getProductTypeParameters/{nameType}")]
        [HttpGet]
        public IActionResult getProductTypeParameters([FromRoute] string nameType)
        {
            
            
            if (string.IsNullOrEmpty(nameType))
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the product name - " + nameType});

            try
            {
                var result = this._productTypeService.getProductTypeParameters(nameType);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("getProductTypeInstances/{nameType}")]
        [HttpGet]
        public IActionResult getProductTypeInstances([FromRoute] string nameType, [FromQuery] PaginationFilterDTO filter, [FromQuery] SortOptionsDTO sortOption = null)
        {
            var route = Request.Path.Value;

            if (string.IsNullOrEmpty(nameType))
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the product name - " + nameType });

            try
            {
                var result  = this._productTypeService.getProductTypeInstances(nameType, filter, route, sortOption, null);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        [Route("getProductTypeInstances2")]
        [HttpPost]
        public IActionResult getProductTypeInstances2([FromBody] getProductTypeInstancesRequest request)
        {
            var route = Request.Path.Value;

            if (string.IsNullOrEmpty(request.NameProductType))
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the product name - " + request.NameProductType });

            try
            {
                var result = this._productTypeService.getProductTypeInstances(request.NameProductType,
                    request.PageFilter, route, request.SortOption, request.Filter);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        // GET: api/GetOtherProductTypes
        [HttpGet]
        [Route("GetOtherProductTypes")]
        public IEnumerable<ProductType> GetOtherProductTypes()
        {
            // type.Id = 1 штока хромированные type.Id = 2 трубы хонингованные
            return _context.ProductTypes.Skip(2);
        }

        // GET: api/ProductTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productType = await _context.ProductTypes.FindAsync(id);

            if (productType == null)
            {
                return NotFound();
            }

            return Ok(productType);
        }

        // PUT: api/ProductTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductType([FromRoute] int id, [FromBody] ProductType productType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productType.Id)
            {
                return BadRequest();
            }

            _context.Entry(productType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTypeExists(id))
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

        // POST: api/ProductTypes
        [HttpPost]
        public async Task<IActionResult> PostProductType([FromBody] ProductType productType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductType", new { id = productType.Id }, productType);
        }

        // DELETE: api/ProductTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();

            return Ok(productType);
        }

        private bool ProductTypeExists(int id)
        {
            return _context.ProductTypes.Any(e => e.Id == id);
        }
    }
}