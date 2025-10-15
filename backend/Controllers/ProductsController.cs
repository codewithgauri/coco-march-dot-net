using Microsoft.AspNetCore.Mvc;
using ProductCrudApi.Models;
using ProductCrudApi.Services;

namespace ProductCrudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase



{
    private readonly ProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }





    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        _logger.LogInformation("Getting all products from MongoDB");
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }





    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        _logger.LogInformation("Getting product with id: {Id}", id);
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            _logger.LogWarning("Product with id {Id} not found", id);
            return NotFound(new { message = $"Product with id {id} not found" });
        }

        return Ok(product);
    }



    

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _logger.LogInformation("Creating new product: {ProductName}", product.Name);
        
        product.CreatedAt = DateTime.UtcNow;
        var createdProduct = await _productService.CreateAsync(product);

        _logger.LogInformation("Product created with id: {Id}", createdProduct.Id);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }




    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, Product product)
    {
        var existingProduct = await _productService.GetByIdAsync(id);
        if (existingProduct == null)
        {
            _logger.LogWarning("Product with id {Id} not found for update", id);
            return NotFound(new { message = $"Product with id {id} not found" });
        }

        _logger.LogInformation("Updating product with id: {Id}", id);
        product.Id = id;
        var updated = await _productService.UpdateAsync(id, product);

        if (updated)
        {
            _logger.LogInformation("Product with id {Id} updated successfully", id);
            return NoContent();
        }

        return BadRequest(new { message = "Failed to update product" });
    }





    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        _logger.LogInformation("Deleting product with id: {Id}", id);
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Product with id {Id} not found for deletion", id);
            return NotFound(new { message = $"Product with id {id} not found" });
        }

        _logger.LogInformation("Product with id {Id} deleted successfully", id);
        return NoContent();
    }
}
