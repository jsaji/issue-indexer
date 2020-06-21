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
    public class ProjectMembersController : ControllerBase
    {
        private readonly IssueIndexerContext _context;

        public ProjectMembersController(IssueIndexerContext context)
        {
            _context = context;
        }

        // GET: api/ProjectMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectMember>>> GetProjectMembers()
        {
            return await _context.ProjectMembers.ToListAsync();
        }

        // GET: api/ProjectMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectMember>> GetProjectMember(uint id)
        {
            var projectMember = await _context.ProjectMembers.FindAsync(id);

            if (projectMember == null)
            {
                return NotFound();
            }

            return projectMember;
        }

        // PUT: api/ProjectMembers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectMember(uint id, ProjectMember projectMember)
        {
            if (id != projectMember.Id)
            {
                return BadRequest();
            }

            _context.Entry(projectMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectMemberExists(id))
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

        // POST: api/ProjectMembers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ProjectMember>> PostProjectMember(ProjectMember projectMember)
        {
            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectMember", new { id = projectMember.Id }, projectMember);
        }

        // DELETE: api/ProjectMembers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProjectMember>> DeleteProjectMember(uint id)
        {
            var projectMember = await _context.ProjectMembers.FindAsync(id);
            if (projectMember == null)
            {
                return NotFound();
            }

            _context.ProjectMembers.Remove(projectMember);
            await _context.SaveChangesAsync();

            return projectMember;
        }

        private bool ProjectMemberExists(uint id)
        {
            return _context.ProjectMembers.Any(e => e.Id == id);
        }
    }
}
