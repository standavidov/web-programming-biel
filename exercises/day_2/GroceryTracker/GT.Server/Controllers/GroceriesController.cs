using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GT.Models;
using GT.Server.Data;

namespace GT.Server.Controllers
{
    [EnableCors]
    public class GroceriesController : ODataController
    {
        private readonly GroceryContext _context;

        public GroceriesController(GroceryContext context)
        {
            _context = context;
        }

        // GET: api/Grocery
        [EnableQuery]
        [HttpGet("odata/groceries")]
        public IEnumerable<Grocery> GetGroceryList()
        {
            return _context.GroceryList;
        }

        // GET: api/Grocery(5)
        [EnableQuery]
        [HttpGet("odata/groceries({id})")]
        public async Task<IActionResult> GetGrocery(int key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var grocery = await _context.GroceryList.FindAsync(key);

            if (grocery == null)
            {
                return NotFound();
            }

            return Ok(grocery);
        }

        // PATCH: api/Grocery(5)
        [HttpPatch("odata/groceries({id})")]
        public async Task<IActionResult> Patch([FromODataUri] int id, [FromBody]Grocery grocery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != grocery.Id)
            {
                return BadRequest();
            }

            var check = _context.GroceryList.Where(item => item.Id == grocery.Id).First();

            if (!check.Expire && grocery.Expire)
            {
                check.MarkExpire();
            }

            check.Name = grocery.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroceryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Grocery
        [HttpPost("odata/groceries")]
        public async Task<IActionResult> Post([FromBody] Grocery grocery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.GroceryList.Add(grocery);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGrocery", new { id = grocery.Id }, grocery);
        }

        // DELETE: api/Grocery(5)
        [HttpDelete("odata/groceries({id})")]
        public async Task<IActionResult> Delete([FromODataUri] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var grocery = await _context.GroceryList.FindAsync(id);
            if (grocery == null)
            {
                return NotFound();
            }

            _context.GroceryList.Remove(grocery);
            await _context.SaveChangesAsync();

            return Ok(grocery);
        }

        private bool GroceryExists(int id)
        {
            return _context.GroceryList.Any(e => e.Id == id);
        }
    }
}