using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
            [FromQuery]ProductSpecParams specParams)
        {
            var spec = new ProductSpecification(specParams);
                
            return await CreatePagedResult(
                repo, 
                spec, 
                specParams.PageIndex, 
                specParams.PageSize);            
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetByIdAsync(id);

            if(product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.Add(product);
            if(await repo.SaveAllAsync())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Cannot create this product!");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {            
            if(id != product.Id || !ProductExists(id))
            {
                return BadRequest("Cannot update this product!");
            }

            repo.Update(product);

            if(await repo.SaveAllAsync())
            {
                return NoContent();
            }        

            return BadRequest("Cannot update this product!");    
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {            
            var product = await repo.GetByIdAsync(id);

            if(product == null)
            {
                return NotFound();
            }

            repo.Delete(product);
            
            if(await repo.SaveAllAsync())
            {
                return NoContent();
            } 

            return BadRequest("Cannot delete this product!");  
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec = new BrandListSpecification();
            var brands = await repo.ListAsync(spec);

            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();
            var types = await repo.ListAsync(spec);

            return Ok(types);
        }

        private bool ProductExists(int id)
        {
            return repo.Exists(id);
        }
    }
}