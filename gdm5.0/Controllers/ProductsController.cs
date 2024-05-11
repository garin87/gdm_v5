using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Requests.Product;
using gdm5._0.Responses;
using gdm5._0.Services;
using gdm5._0.Services.Interfaces;
using gdm5._0.Shared.Constants;
using gdm5._0.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0
{
    [Route("api/product")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(DataContext context, IProductService ProductService, IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
            _productService = new ProductService(context);
        }

        // GET: api/Products/GetAll
        [Authorize]
        [HttpGet]
        public IActionResult GetAll([FromServices] IProductService productService)
        {
            
            return Ok(productService.GetAll());
        }

        // GET: api/Products/GetProduct/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id, [FromServices] IProductService productService)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await productService.GetItem(id);

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        // POST: api/Products/PostProduct
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product, [FromServices] IProductService productService)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productNew = await productService.AddItem(product);

            return Ok(productNew);
        }

        // DELETE: api/Products/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.DeleteItem(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        //Post: api/Products/PutProduct
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PutProduct([FromBody] updateProductInstancesRequest product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (product.ProductId != 0)
            {
                return BadRequest();
            }

            await _productService.UpdateProduct(product, CurrentUserName);

            return NoContent();
        }

        [Authorize]
        [Route("getInstancesOfProductParameter")]
        [HttpPost]
        public IActionResult getInstancesOfProductParameter([FromBody] getInstancesOfProductParameterRequest parameterOption)
        {
            if (string.IsNullOrEmpty(parameterOption.NameParameter))
                return BadRequest(new { IsSuccess = false,
                       Message = "Incorrect the product name parameter - " + parameterOption.NameParameter });

            try
            {
                var result = _productService.GetInstancesOfProductParameterUpdated(parameterOption);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        // add Add Other Products
        [Authorize]
        [HttpPost]
        [Route("AddNewProducts")]
        public IActionResult AddOtherProducts(addNewProductTypeRequest Product)
        {
            if (Product == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = _productService.AddOtherProducts(Product, CurrentUserName);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name.Value + " has cteated."});
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("AddInstanceProduct")]
        public IActionResult AddInstanceProduct(ProductNewDTO ProductDTO)
        {

            if (ProductDTO == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                var result = _productService.AddInstanceProduct(ProductDTO, CurrentUserName);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name.Value + " has cteated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("UpdateProductInstance")]
        [HttpPost]
        public async Task<IActionResult> UpdateProductInstance([FromBody]  updateProductInstancesRequest ProductDTO)
        {

            try
            {
                var result = await _productService.UpdateProduct(ProductDTO, CurrentUserName);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.name.Value + " has updated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProductInstance(int id)
        {
            try
            {
                var result = await _productService.DeleteProductInstance(id, CurrentUserName);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name + " Product Number " + result.ProductNumber + " has deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getProductManufacturers")]
        [HttpGet]
        public IActionResult getProductManufacturers()
        {
            try
            {
                return Ok(this._productService.GetProductManufacturers());
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getManufacturersByProductName/{productName}")]
        [HttpGet]
        public IActionResult GetProductManufacturers([FromRoute] string productName)
        {
            try
            {
                return Ok(this._productService.GetProductManufacturers(productName));
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getProductSuppliers")]
        [HttpGet]
        public IActionResult getProductSuppliers()
        {
            try
            {
                return Ok(this._productService.GetProductSuppliers());
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getProductSuppliersByProductName/{productName}")]
        [HttpGet]
        public IActionResult getProductSuppliers([FromRoute] string productName)
        {
            try
            {
                return Ok(_productService.GetProductSuppliers(productName));
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Authorize]
        [Route("getParametersByName")]
        [HttpPost]
        public IActionResult getParametersByName([FromBody] getInstancesOfProductParameterRequest parameterOption)
        {
            if (string.IsNullOrEmpty(parameterOption.NameParameter))
                return BadRequest(new {
                    IsSuccess = false,
                    Message = "Incorrect the product name parameter - " + parameterOption.NameParameter
                });

            try
            {
                return Ok("");
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        [Authorize]
        [Route("loadReport")]
        [HttpGet]
        public ApplicationResponse GenerateReportAsync()
        {
            return GenerateReportBaseAsync();
        }

        protected ApplicationResponse GenerateReportBaseAsync()
        {
            try
            {
                var bytes =  _productService.GeneratePDF();
                return new ApplicationResponseGeneric<FileContentResult> { Data = File(bytes, "application/pdf") };
            }
            catch (Exception ex)
            {
                return new ApplicationResponse(StatusCodeEnum.Unknown);
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