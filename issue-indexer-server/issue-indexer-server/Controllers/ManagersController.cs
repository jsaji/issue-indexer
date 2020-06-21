using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using issue_indexer_server.Models;

namespace issue_indexer_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private readonly IssueIndexerContext _context;

        public ManagersController(IssueIndexerContext context)
        {
            _context = context;
        }

        // GET: api/Managers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manager>>> GetManagers()
        {
            return await _context.Managers.ToListAsync();
        }

        // GET: api/Managers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Manager>> GetManager(uint id)
        {
            var manager = await _context.Managers.FindAsync(id);

            if (manager == null)
            {
                return NotFound();
            }

            return manager;
        }

        // PUT: api/Managers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManager(uint id, Manager manager)
        {
            if (id != manager.Id)
            {
                return BadRequest();
            }

            _context.Entry(manager).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagerExists(id))
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

        // POST: api/Managers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Manager>> PostManager(Manager manager)
        {
            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetManager", new { id = manager.Id }, manager);
        }

        // DELETE: api/Managers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Manager>> DeleteManager(uint id)
        {
            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();

            return manager;
        }

        private bool ManagerExists(uint id)
        {
            return _context.Managers.Any(e => e.Id == id);
        }
    }
}
