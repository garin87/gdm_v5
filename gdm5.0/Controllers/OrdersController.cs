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
using gdm5._0.Controllers.Base;
using System.Linq;
using gdm5._0.Shared.Constants;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace gdm5._0.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : WarehouseController
    {
        public readonly DataContext _context;
        public OrderService _orderService;

        public OrdersController(DataContext context, IHttpContextAccessor httpContextAccessor,
             IProductService ProductService, IUriService uriService):base(httpContextAccessor)
        {
               _context = context;
               _orderService = new OrderService(_context, ProductService, uriService);
        }
  
        // GET: api/Orders
        [HttpGet]
        [Authorize]
        public IEnumerable<Order> GetOrders()
        {
            return _orderService.GetAll();
        }

        // GET: api/Orders/5
        [Authorize]
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

        // POST: api/Orders/addOrderProduct
        [Authorize]
        [Route("addOrderProduct")]
        [HttpPost]
        public async Task<IActionResult> addOrderProduct(AddOrderRequest AddOrderRequest)
        {
            try
            {
                await _orderService.AddOrder(AddOrderRequest, CurrentUserName);
                return Ok(new { IsSuccess = true, Message = "Success: Order has added." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        // POST: api/Orders/addToCartOrder
        [Authorize]
        [Route("addToCartOrder")]
        [HttpPost]
        public async Task<IActionResult> AddToCartOrder(AddOrderRequest AddOrderRequest)
        {
            try
            {
                await _orderService.AddToCartOrder(AddOrderRequest, CurrentUserName);
                return Ok(new { IsSuccess = true, Message = "Success: Order has added to cart." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        // POST: api/Orders/addOrderProduct
        [Authorize]
        [Route("addOrderProductList")]
        [HttpPost]
        public async Task<IActionResult> addOrderProductList(addOrderListProductRequest AddOrderRequest)
        {
            try
            {
                await _orderService.addOrderProductList(AddOrderRequest, CurrentUserId ?? 0, CurrentUserName);
                return Ok(new { IsSuccess = true, Message = "Success: Order has added." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        [Authorize]
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

        [Authorize]
        [Route("getOrderProductList")]
        [HttpPost]
        public IActionResult getOrderProductList(getOrdersRequest request)
        {
            var route = Request.Path.Value;
            try
            {
                var result = this._orderService.getOrderProducts(request.Name, request.PageFilter, route, request.SortOption, request.Filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getCartOrderProducts")]
        [HttpGet]
        public IActionResult getCartOrderProducts()
        {
            var route = Request.Path.Value;
            try
            {
                var t = CurrentUserName;
                var rt = CurrentUserId;
                var result = this._orderService.getCartOrderProducts(route, CurrentUserId ?? 0);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("saveOrderCart")]
        [HttpGet]
        public async Task<IActionResult> saveOrderCart()
        {
            try
            {
                await this._orderService.saveOrderCart(CurrentUserName, CurrentUserId ?? 0);

                return Ok(new { IsSuccess = true, Message = "Success: Order has saved." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [Route("GetCartOrderCount")]
        [HttpGet]
        public IActionResult GetCartOrderCount()
        {
            try
            {
                return Ok(_orderService.GetCartOrderCount(CurrentUserId ?? 0));
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getOrderNameCompanies")]
        [HttpGet]
        public IActionResult getOrderNameCompanies()
        {
            try
            {
                return Ok(_orderService.getOrderNameCompanies());
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [Route("loadOrederReport")]
        [HttpPost]
        public IActionResult loadOrderReport(loadOrderReportRequest request)
        {
            var route = Request.Path.Value;
            try
            {
                var result = this._orderService.LoadOrderReport(request);  //  this._orderService.getOrderProducts(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getProcessingOrderInfo")]
        [HttpGet]
        public IActionResult GetProcessingOrderInfo()
        {
            try
            {
                return Ok(_orderService.GetProcessingOrderInfo(CurrentUserId ?? 0));
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        private string CurrentUserName {
            get {
                //  var companyIdClaim2 = _httpContextAccessor.HttpContext.User.Claims.ToList();
                //  var userName =_httpContextAccessor.HttpContext.User?.Identity?.Name;
                if (_httpContextAccessor?.HttpContext == null)
                {
                    return "";
                }

                if (_httpContextAccessor?.HttpContext?.User == null)
                {
                    return "";
                }

                var userNameClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserName");

                if (userNameClaim == null)
                {
                    return "";
                }

                return string.IsNullOrEmpty(userNameClaim?.Value) ? "" : userNameClaim?.Value;
            }
        }

        private int? CurrentUserId {
            get {
                if (_httpContextAccessor?.HttpContext == null)
                {
                    return 0;
                }

                if (_httpContextAccessor?.HttpContext?.User == null)
                {
                    return 0;
                }
                var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimsConstants.UserId);

                if (userIdClaim == null)
                {
                    return 0;
                }


                return userIdClaim != null ? int.Parse(userIdClaim?.Value) : 0;
            }
        }
    }
}