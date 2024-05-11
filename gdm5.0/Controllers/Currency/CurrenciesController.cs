using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Requests.Customer;
using gdm5._0.Requests.Product;
using gdm5._0.Services;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace gdm5._0
{
    // [Authorize(Roles = "admin")] 
    [Route("api/сurrency")]
    [ApiController]
    public class CurrenciesController : Controller
    {
        private readonly ICurrencyService _сurrencyService;
        public CurrenciesController(ICurrencyService сurrencyService) 
        {
            _сurrencyService = сurrencyService;
        }


        // GET: api/сurrency/GetAll
        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            
            return Ok(_сurrencyService.GetAll());
        }

        // GET: api/сurrency/GetСurrency/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetСurrency(int id)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _сurrencyService.GetItem(id);

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        // POST: api/сurrency/AddСurrency
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddСurrency(Currency сurrency)
        {    
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productNew = await _сurrencyService.AddItem(сurrency);

            return Ok(productNew);
        }

        [Authorize]
        [Route("DeleteСurrency/{id}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteСurrency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _сurrencyService.DeleteItem(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [Authorize]
        [Route("GetNamesCurrencies")]
        [HttpGet]
        public IActionResult GetNamesCurrencies()
        {
            try
            {
                var result = this._сurrencyService.GetNamesCurrencies();
                return Ok(result);
            }
            catch (ApplicationException ex) 
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getCurrencyByNameCurrency/{nameCurrency}")]
        [HttpGet]
        public IActionResult getCurrencyByNameCurrency([FromRoute] string nameCurrency)
        {
            try
            {
                var result = this._сurrencyService.GetCurrencyByName(nameCurrency);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("AddNewCurrency")]
        public async Task<IActionResult> AddNewCurrency(addCurrencyRequest currency)
        {

            if (currency == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = await _сurrencyService.AddNewCurrency(currency);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.CurrencyName + " has cteated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("UpdateCurrency")]
        public async Task<IActionResult> UpdateCurrency(UpdateCurrencyRequest currency)
        {

            if (currency == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = await _сurrencyService.UpdateCurrency(currency);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.CurrencyName + " has cteated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

    }
}