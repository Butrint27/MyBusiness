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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProductsAsync()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProductByIdAsync(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDTO>> CreateProductAsync(ProductDTO productDTO)
    {
        try
        {
            var createdProduct = await _productService.CreateProductAsync(productDTO);
            return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createdProduct.ProductId }, createdProduct);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<ProductDTO>> UpdateProductAsync(ProductDTO productDTO)
    {
        var existingProduct = await _productService.UpdateProductAsync(productDTO);
        if (existingProduct == null)
        {
            return NotFound();
        }

        try
        {
            var updatedProduct = await _productService.UpdateProductAsync(productDTO);
            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductAsync(int id)
    {
        var deleted = await _productService.DeleteProductAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
    }
} 