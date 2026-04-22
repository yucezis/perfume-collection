using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Perfume.Models;
using Perfume.Data;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace Perfume.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BrandController : ControllerBase
    {

        private readonly PerfumeDbContext _context;

        public BrandController(PerfumeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous] 
        public ActionResult<IEnumerable<Brand>> GetAll()
        {
            var brands = _context.Brands.ToList();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<Brand> GetById(int id)
        {
            var brand = _context.Brands.Find(id);
            if (brand == null) { return NotFound();  }
            else return Ok(brand);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult<Brand> Create(Brand newBrand)
        {
            _context.Brands.Add(newBrand);
            _context.SaveChanges();
            return Ok(newBrand);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, Brand updatedBrand)
        {
            
            if(id != updatedBrand.Id) { return BadRequest(); }

            var brand = _context.Brands.Find(id);

            if(brand == null) { return NotFound(); }

            brand.Name = updatedBrand.Name;
            brand.Country = updatedBrand.Country;
            _context.SaveChanges();
            return Ok(brand);

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id) 
        { 
            var brand = _context.Brands.Find(id);
            if (brand == null) { return NotFound(); }

            _context.Brands.Remove(brand);
            _context.SaveChanges();
            return NoContent();

        }
    }
}
