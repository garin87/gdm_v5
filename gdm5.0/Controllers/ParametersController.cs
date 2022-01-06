using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.Services;
using gdm5._0.DTO;

namespace gdm5._0
{
    [Route("api/parameter")]
    [ApiController]
    public class ParametersController : ControllerBase
    {
        private readonly DataContext _context;
        private ParameterService _parameterService;

        public ParametersController(DataContext context)
        {
            _context = context;
            _parameterService = new ParameterService(_context);

        }
     
        // GET: api/Parameters
        [HttpGet]
        public IActionResult GetParameters()
        {
            return Ok( _parameterService.GetAll()); 
        }

        // GET: api/Parameters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParameter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parameter = await _parameterService.GetItem(id);

            if (parameter == null)
            {
                return NotFound();
            }

            return Ok(parameter);
        }

 
        [HttpPost]
        [Route("UpdateProductTypeParameters")]
        public IActionResult UpdateProductTypeParameters(ProductNewDTO ProductDTO)
        {
            var f = ProductDTO;

            if (ProductDTO == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = this._parameterService.UpdateProductTypeParameters(ProductDTO);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name.Value + " has updated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutParameter([FromRoute] int id, [FromBody] Parameter parameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != parameter.Id)
            {
                return BadRequest();
            }

            await _parameterService.UpdateParameter(id, parameter);

            return NoContent();
        }

        // POST: api/Parameters
        [HttpPost]
        public async Task<IActionResult> PostParameter([FromBody] Parameter parameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parameterNew = await _parameterService.AddItem(parameter);

            return Ok(parameterNew);
        }

        // DELETE: api/Parameters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParameter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parameter = await _parameterService.DeleteItem(id);

            if (parameter == null)
            {
                return NotFound();
            }

            _context.Parameters.Remove(parameter);
            await _context.SaveChangesAsync();

            return Ok(parameter);
        }

      
    }
}