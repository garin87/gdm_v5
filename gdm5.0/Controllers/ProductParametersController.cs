using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.Services;
using gdm5._0.DTO;
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
        [HttpGet]
        public IEnumerable<ProductParameter> GetProductParameters()
        {
            return _context.ProductParameters;
        }

        // GET: api/ProductParameters/5
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

        //api/ProductParameters/GetProductParameters/4
        [HttpGet]
        [Route("GetProductParameters/{id}")]
        public Task<IEnumerable<ProductParameter>> GetProductParameters(long id)
        {
            return  _productParameterService.GetProductParameters(id);
        }

        [HttpGet]
        [Route("GetProductDiameters/{id}")]
        public Task<IEnumerable<DiameterDTO>> GetProductDiameters(int id, [FromQuery]string param, [FromQuery]int paramId)
        {
            return _productParameterService.GetProductDiameters(id, param, paramId);
        }

        //api/ProductParameters/1?param=Стандарт&paramId=2&paramDiameterId=4&diameter=20
        [HttpGet]
        [Route("GetProductByDiameter/{typeId}")]
        public Task<IEnumerable<ProductDTO>> GetProductDiameters(int typeId, string param, int paramId, int paramDiameterId, string diameter)
        {
            return _productParameterService.GetProductsByDiameter(typeId, param, paramId, paramDiameterId, diameter);
        }

        // get other product
        //api/GetOtherProductByType/typeId = 3;
        [HttpGet]
        [Route("GetOtherProductByType/{typeId}")]
        public Task<IEnumerable<ProductDTO>> GetOtherProductByType(int typeId)
        {
            return _productParameterService.GetOtherProducts(typeId);
        }

        [HttpGet]
        [Route("GetSortProductParameters/{id}")]
        public Task<IEnumerable<ProductParameter>> GetSortProductParameters(long id)
        {
            return _productParameterService.GetSortProductParameters(id);
        }

        [HttpGet]
        [Route("GetSumSteelBars/{id}")]
        public async Task<double> GetSumSteelBars(int value)
        {
            return await _productParameterService.GetSumSteelBars(value);
        }

        [HttpGet]
        [Route("GetSumTubes/{id}")]
        public async Task<double> GetSumTubes(int value)
        {
            return await _productParameterService.GetSumTubes(value);
        }

        // api/ProductParameters/GetSumQuantityByParam/1?param=Стандарт&paramId=1&paramDiameterId=4&diameter=20
        [HttpGet]
        [Route("GetSumQuantityByParam/{typeId}")]
        public async Task<double> GetSumQuantityByParam(int typeId, string param, int paramId, int paramDiameterId, string diameter)
        {
            return await _productParameterService.GetSumQuantityByParam(typeId, param, paramId, paramDiameterId, diameter);
        }

        // api/ProductParameters/GetSumPriceByParam/1?param=Стандарт&paramId=1&paramDiameterId=4&diameter=20
        [HttpGet]
        [Route("GetSumPriceByParam/{typeId}")]
        public async Task<double> GetSumPriceByParam(int typeId, string param, int paramId, int paramDiameterId, string diameter)
        {
            return await _productParameterService.GetSumPriceByParam(typeId, param, paramId, paramDiameterId, diameter);
        }

    }

    internal class _ProductParameterService : ProductParameterService
    {
        public _ProductParameterService(DataContext context) : base(context)
        {
        }
    }
}