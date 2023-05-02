using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Requests.Product;
using gdm5._0.Services;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace gdm5._0
{
    // [Authorize(Roles = "admin")] 
    [Route("api/product")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        private readonly IProductService _productService;


        public ProductsController(DataContext context, IProductService ProductService, IHttpContextAccessor httpContextAccessor) 
        {
            _context = context;
            _productService = new ProductService(context, httpContextAccessor);
        }

 
        // GET: api/Products/GetAll
        [HttpGet]
        public IActionResult GetAll([FromServices] IProductService productService)
        {
            
            return Ok(productService.GetAll());
        }

        // GET: api/Products/GetProduct/5
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

            await _productService.UpdateProduct(product);

            return NoContent();
        }

        [Route("getInstancesOfProductParameter")]
        [HttpGet]
        public IActionResult getInstancesOfProductParameter([FromQuery] ParameterDTO parameterOption)
        {
            string nameParam = parameterOption.Name;
            string nameType = parameterOption.ProductTypeName;
            bool isParameter = parameterOption.isParameter;

            if (string.IsNullOrEmpty(nameParam))
                return BadRequest(new { IsSuccess = false,
                       Message = "Incorrect the product name parameter - " + nameParam });

            try
            {
                var result = _productService.getInstancesOfProductParameter(nameType, nameParam, isParameter);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }
        
        //GET: api/Products/GetProducts/1
        [HttpGet]
        [Route("GetProducts/{id}")]
        public async Task<IEnumerable<ProductDTO>> GetProducts([FromRoute] int id, [FromServices] IProductService productService)
        {
            return await productService.GetProducts(id);
        }

        [HttpPost]
        [Route("AddProducts")]
        public async Task<ProductDTO> AddProducts([FromBody] ProductDTO ProductDTO)
        {
            return await _productService.AddProducts(ProductDTO);
        }

        // add Add Other Products
        [HttpPost]
        [Route("AddNewProducts")]
        public IActionResult AddOtherProducts(addNewProductTypeRequest Product)
        {

            if (Product == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                
                var result = _productService.AddOtherProducts(Product);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name.Value + " has cteated."});
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("AddInstanceProduct")]
        public IActionResult AddInstanceProduct(ProductNewDTO ProductDTO)
        {

            if (ProductDTO == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect the data" });

            try
            {
                for (var index = 1; index < 3; index++)
                {
                    
                    ProductDTO.ProductNumber.Value = ProductDTO?.ProductNumber == null ? " t-" + index : ProductDTO?.ProductNumber.Value + " t-" + index;
                    _productService.AddInstanceProduct(ProductDTO);
                }
                var result = _productService.AddInstanceProduct(ProductDTO);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name.Value + " has cteated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [Route("UpdateProductInstance")]
        [HttpPost]
        public async Task<IActionResult> UpdateProductInstance([FromBody]  updateProductInstancesRequest ProductDTO)
        {

            try
            {
                var result = await _productService.UpdateProduct(ProductDTO);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.name.Value + " has updated." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProductInstance(int id)
        {

            try
            {
                var result = await _productService.DeleteProductInstance(id);
                return Ok(new { IsSuccess = true, Message = "Success: " + result.Name + " Product Number " + result.ProductNumber + " has deleted." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        [Route("getProductManufacturers")]
        [HttpGet]
        public IActionResult getProductManufacturers()
        {
            try
            {
                return Ok(this._productService.GetProductManufacturers());
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("getManufacturersByProductName/{productName}")]
        [HttpGet]
        public IActionResult GetProductManufacturers([FromRoute] string productName)
        {
            try
            {
                return Ok(this._productService.GetProductManufacturers(productName));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("getProductSuppliers")]
        [HttpGet]
        public IActionResult getProductSuppliers()
        {
            try
            {
                return Ok(this._productService.GetProductSuppliers());
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [Route("getProductSuppliersByProductName/{productName}")]
        [HttpGet]
        public IActionResult getProductSuppliers([FromRoute] string productName)
        {
            try
            {
                return Ok(this._productService.GetProductSuppliers(productName));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        //  [Authorize(Roles = "Admin")]
        //[HttpGet]
        //[Route("SortProducts/{id}")]
        //public async Task<IEnumerable<ProductDTO>> SortProducts([FromRoute] int id)
        //{
        //    return await _productService.SortProducs(id);
        //}

        //[HttpGet]
        //[Route("SortProducsByParameters/{id}")]
        //public async Task<IEnumerable<ProductDTO>> SortProducsByParameters([FromRoute] int id, [FromQuery] bool StateOrder)
        //{
        //    return await _productService.SortProducsByParameters(id, StateOrder);
        //}

        //[HttpGet]
        //[Route("GetProductParam/{id}")]
        //public async Task<IEnumerable<ProductDTO>> GetProductParam([FromRoute] int id)
        //{
        //    return await _productService.GetProductParam(id);
        //}

        //[HttpGet]
        //[Route("GetParamForOrder/{id}")]
        //public async Task<IEnumerable<ProductOrderDTO>> GetParamForOrder([FromRoute] int id)
        //{
        //    return await _productService.GetParamForOrder(id);
        //}

    }
}