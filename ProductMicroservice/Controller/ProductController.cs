using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ProductMicroservice.Services;

namespace MyBusiness.ProductMicroservice.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdProduct = await _productService.CreateProductAsync(productDTO);
            return Ok(createdProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(int productId)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] ProductDTO productDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var existingProduct = await _productService.GetProductByIdAsync(productDTO.ProductId);
            if (existingProduct == null)
            {
                return NotFound();
            }

            var updatedProduct = await _productService.UpdateProductAsync(productDTO);

            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(productId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    }
}