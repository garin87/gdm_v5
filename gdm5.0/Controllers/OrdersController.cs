using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using gdm5._0.Models;
using gdm5._0.Services;
using gdm5._0.Requests.Product;
using gdm5._0.Services.Interfaces;
using gdm5._0.Requests.Order;

namespace gdm5._0.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;

        private OrderService _orderService;

        public OrdersController(DataContext context, IHttpContextAccessor httpContextAccessor,
             IProductService ProductService, IUriService uriService)
        {
            _context = context;
            _orderService = new OrderService(_context, httpContextAccessor, ProductService, uriService);
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

        
        // POST: api/Orders/addOrderProduct
        [Route("addOrderProduct")]
        [HttpPost]
        public async Task<IActionResult> addOrderProduct(AddOrderRequest AddOrderRequest)
        {
            try
            {
                await _orderService.AddOrder(AddOrderRequest);
                return Ok(new { IsSuccess = true, Message = "Success: Order has added." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        // POST: api/Orders/addToCartOrder
        [Route("addToCartOrder")]
        [HttpPost]
        public async Task<IActionResult> AddToCartOrder(AddOrderRequest AddOrderRequest)
        {
            try
            {
                await _orderService.AddToCartOrder(AddOrderRequest);
                return Ok(new { IsSuccess = true, Message = "Success: Order has added to cart." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        // POST: api/Orders/addOrderProduct
        [Route("addOrderProductList")]
        [HttpPost]
        public async Task<IActionResult> addOrderProductList(addOrderListProductRequest AddOrderRequest)
        {
            try
            {
                await _orderService.addOrderProductList(AddOrderRequest);
                return Ok(new { IsSuccess = true, Message = "Success: Order has added." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        [Route("getOrderProduct")]
        [HttpPost]
        public IActionResult getOrderProduct(getOrdersRequest request)
        {
            var route = Request.Path.Value;
            try
            {
                var result = this._orderService.getOrderProduct(request.Name,
                    request.PageFilter, route, request.SortOption, request.Filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("getOrderProductList")]
        [HttpPost]
        public IActionResult getOrderProductList(getOrdersRequest request)
        {
            var route = Request.Path.Value;
            try
            {
                var result = this._orderService.getOrderProducts(request.Name,
                    request.PageFilter, route, request.SortOption, request.Filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("getCartOrderProducts")]
        [HttpGet]
        public IActionResult getCartOrderProducts()
        {
            var route = Request.Path.Value;
            try
            {
                var result = this._orderService.getCartOrderProducts(route);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("saveOrderCart")]
        [HttpGet]
        public async Task<IActionResult> saveOrderCart()
        {
            try
            {
                await this._orderService.saveOrderCart();

                return Ok(new { IsSuccess = true, Message = "Success: Order has saved." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("cancelOrderCart")]
        [HttpGet]
        public IActionResult cancelOrderCart()
        {
            try
            {
                this._orderService.cancelOrderCart();

                return Ok(new { IsSuccess = true, Message = "Success: Order canceled." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("deleteOrderProduct/{id}")]
        public IActionResult deleteOrderProduct(int id)
        {
            try
            {
                this._orderService.deleteOrderProduct(id);

                return Ok(new { IsSuccess = true, Message = "Success: Order deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }
        [Route("GetCartOrderCount")]
        [HttpGet]
        public IActionResult GetCartOrderCount()
        {
            try
            {
                return Ok(this._orderService.GetCartOrderCount());
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        // GET: api/Orders
        //[HttpGet]
        //[Route("GetOrders")]
        //public Task<IEnumerable<Order>> GetOrderProd()
        //{
        //    return _orderService.GetOrders();
        //}
        // GET: api/Orders/GetOrderProducts
        //[HttpGet]
        //[Route("GetOrderProducts")]
        //public Task<IEnumerable<OrderPDTO>> GetOrderProducts()
        //{
        //    return _orderService.GetOrderProduct();
        //}

        // GET: api/Orders/GetOrderByNameCompany/{nameCompany}
        //[HttpGet]
        //[Route("GetOrderByNameCompany/{nameCompany}")]
        //public Task<IEnumerable<OrderPDTO>> GetOrderByNameCompany(string nameCompany)
        //{
        //    return _orderService.GetOrderByNameCompany(nameCompany);
        //}

        [Route("getOrderNameCompanies")]
        [HttpGet]
        public IActionResult getOrderNameCompanies()
        {
            try
            {
                return Ok(this._orderService.getOrderNameCompanies());
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("getOrderNameCompaniesByNameCompany")]
        [HttpGet]
        public IActionResult getOrderNameCompanies([FromRoute] string nameCompany)
        {
            try
            {
                return Ok(_orderService.getOrderNameCompanies(nameCompany));
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        [HttpDelete]
        [Route("deleteCartProduct/{id}")]
        public  IActionResult DeleteCartProduct(int id)
        {

            try
            {
                _orderService.DeleteCartProduct(id);
                return Ok(new { IsSuccess = true, Message = "Success: Product has deleted from cart." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("getNamesProduct")]
        [HttpGet]
        public IActionResult getNamesProduct()
        {
            try
            {
                return Ok(this._orderService.getNamesProduct());
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }
       
        // DELETE: api/Orders/DeleteOrders/5
        //[HttpDelete("DeleteOrders/{id}")]
        //public async Task<IActionResult> DeleteOrders([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var order = await _orderService.DeleteOrders(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(order);
        //}
    }
}