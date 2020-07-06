using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using issue_indexer_server.Models;
using issue_indexer_server.Data;

namespace issue_indexer_server.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class UserRelationshipsController : ControllerBase {

        private readonly IssueIndexerContext _context;

        public UserRelationshipsController(IssueIndexerContext context) {
            _context = context;
        }

        // GET: api/UserRelationships
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRelationship>>> GetManagedMembers() {
            return await _context.UserRelationships.ToListAsync();
        }

        // GET: api/UserRelationships/5
        [HttpGet]
        public async Task<ActionResult<UserRelationship>> GetManagedMember(uint? userAId, uint? userBId) {
            if (!userAId.HasValue || !userBId.HasValue) return BadRequest();
            var managedMember = await _context.UserRelationships.FindAsync(userAId.Value, userBId.Value);
            if (managedMember == null) return NotFound();
            return managedMember;
        }

        // PUT: api/UserRelationships/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> PutManagedMember(uint? userAId, uint? userBId, UserRelationship managedMember) {
            if (!userAId.HasValue || !userBId.HasValue) return BadRequest();
            if (!ManagedMemberExists(userAId.Value, userBId.Value)) return NotFound();

             _context.Entry(managedMember).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception) {
                throw;
            }

            return NoContent();
        }

        // POST: api/UserRelationships
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserRelationship>> PostManagedMember(UserRelationship userRelationship) {
            // Checks if the record already exists within the Db
            var result = await (from mm in _context.UserRelationships
                                where mm.UserAId == userRelationship.UserAId
                                && mm.UserBId == userRelationship.UserBId
                                && mm.UserBSuperior == userRelationship.UserBSuperior
                                select mm).FirstOrDefaultAsync();

            // Checks that user and superior exists
            // could be simplified if API knows who's making the call
            var userExists = await Functions.UserExists(_context, userRelationship.UserAId); ;
            var superiorExists = await Functions.UserExists(_context, userRelationship.UserBId);

            if (!userExists || !superiorExists) return BadRequest();

            if (result == null) {
                
                _context.UserRelationships.Add(userRelationship);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetManagedMember", userRelationship);
            } else {
                return NoContent();
            }
            
        }

        // DELETE: api/UserRelationships/5
        [HttpDelete]
        public async Task<ActionResult<UserRelationship>> DeleteManagedMember(uint? userAId, uint? userBId) {
            if (!userAId.HasValue || !userBId.HasValue) return BadRequest();
            var userRelationship = await _context.UserRelationships.FindAsync(userAId.Value, userBId.Value);
            if (userRelationship == null) return NotFound();

            _context.UserRelationships.Remove(userRelationship);
            await _context.SaveChangesAsync();

            return userRelationship;
        }

        private bool ManagedMemberExists(uint userAId, uint userBId) {
            return _context.UserRelationships.Any(e => e.UserAId == userAId && e.UserBId == userBId);
        }
    }
}
