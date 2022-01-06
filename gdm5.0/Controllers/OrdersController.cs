using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.DTO;
using gdm5._0.Services;

namespace gdm5._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;

        private OrderService _orderService;

        public OrdersController(DataContext context)
        {
            _context = context;
            _orderService = new OrderService(_context);
        }
  
        // GET: api/Orders
        [HttpGet]
        public IEnumerable<Order> GetOrders()
        {
            return _orderService.GetAll();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.GetItem(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder([FromRoute] int id, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            await _orderService.UpdateOrder(id, order);

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderNew = await _orderService.AddItem(order);

            return Ok(orderNew);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.DeleteOrder(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        // POST: api/Orders/addOrderProduct
        [HttpPost]
        [Route("addOrderProduct")]
        public async Task<OrderDTO> addOrderProduct(OrderDTO orderDTO)
        {

            var orderNew = await _orderService.AddOrders(orderDTO);

            return orderNew;
        }

        // GET: api/Orders
        [HttpGet]
        [Route("GetOrders")]
        public Task<IEnumerable<Order>> GetOrderProd()
        {
            return _orderService.GetOrders();
        }
        // GET: api/Orders/GetOrderProducts
        [HttpGet]
        [Route("GetOrderProducts")]
        public Task<IEnumerable<OrderPDTO>> GetOrderProducts()
        {
            return _orderService.GetOrderProduct();
        }

        // GET: api/Orders/GetOrderByNameCompany/{nameCompany}
        [HttpGet]
        [Route("GetOrderByNameCompany/{nameCompany}")]
        public Task<IEnumerable<OrderPDTO>> GetOrderByNameCompany(string nameCompany)
        {
            return _orderService.GetOrderByNameCompany(nameCompany);
        }

        // DELETE: api/Orders/DeleteOrders/5
        [HttpDelete("DeleteOrders/{id}")]
        public async Task<IActionResult> DeleteOrders([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.DeleteOrders(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}