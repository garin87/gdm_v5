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

namespace gdm5._0.Controllers
{
    [Route("api/orderProducts")]
    [ApiController]
    public class OrderProductsController : Controller
    {
        private readonly DataContext _context;
        private OrderProductService _orderProductService;

        public OrderProductsController(DataContext context)
        {
            _context = context;
            _orderProductService = new OrderProductService(_context);
        }

        // GET: api/OrderProducts
        [Authorize]
        [HttpGet]
        public IEnumerable<OrderProduct> GetOrderProducts()
        {
            return _orderProductService.GetAll();
        }

        // GET: api/OrderProducts/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderProduct = await _orderProductService.GetItem(id);

            if (orderProduct == null)
            {
                return NotFound();
            }

            return Ok(orderProduct);
        }

        // PUT: api/OrderProducts/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderProduct([FromRoute] int id, [FromBody] OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderProduct.Id)
            {
                return BadRequest();
            }

            await _orderProductService.UpdateOrderProduct(id, orderProduct);

            return NoContent();
        }

        // POST: api/OrderProducts
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostOrderProduct([FromBody] OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderProductNew = await _orderProductService.AddItem(orderProduct);

            return Ok(orderProductNew);
        }

        // DELETE: api/OrderProducts/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderProduct = await _orderProductService.DeleteItem(id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return Ok(orderProduct);
        }

    }
}