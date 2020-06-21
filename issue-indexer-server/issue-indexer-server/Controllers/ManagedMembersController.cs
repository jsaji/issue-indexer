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
    public class ManagedMembersController : ControllerBase
    {
        private readonly IssueIndexerContext _context;

        public ManagedMembersController(IssueIndexerContext context)
        {
            _context = context;
        }

        // GET: api/ManagedMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagedMember>>> GetManagedMembers()
        {
            return await _context.ManagedMembers.ToListAsync();
        }

        // GET: api/ManagedMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagedMember>> GetManagedMember(uint id)
        {
            var managedMember = await _context.ManagedMembers.FindAsync(id);

            if (managedMember == null)
            {
                return NotFound();
            }

            return managedMember;
        }

        // PUT: api/ManagedMembers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManagedMember(uint id, ManagedMember managedMember)
        {
            if (id != managedMember.Id)
            {
                return BadRequest();
            }

            _context.Entry(managedMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagedMemberExists(id))
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

        // POST: api/ManagedMembers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ManagedMember>> PostManagedMember(ManagedMember managedMember)
        {
            _context.ManagedMembers.Add(managedMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetManagedMember", new { id = managedMember.Id }, managedMember);
        }

        // DELETE: api/ManagedMembers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManagedMember>> DeleteManagedMember(uint id)
        {
            var managedMember = await _context.ManagedMembers.FindAsync(id);
            if (managedMember == null)
            {
                return NotFound();
            }

            _context.ManagedMembers.Remove(managedMember);
            await _context.SaveChangesAsync();

            return managedMember;
        }

        private bool ManagedMemberExists(uint id)
        {
            return _context.ManagedMembers.Any(e => e.Id == id);
        }
    }
}
