using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using issue_indexer_server.Models;
using issue_indexer_server.Data;
using issue_indexer_server.Models.DTO;

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

        // POST: api/ProjectMembers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ProjectMember>> PostProjectMember(ProjectMemberList projectMembers)
        {
            // Basic data check
            if (projectMembers == null || projectMembers.list.Count == 0) return BadRequest();

            // Illegal operation to assign members between different projects in a single call
            var projectIds = projectMembers.list.Select(pm => pm.ProjectId).Distinct().ToList();
            if (projectIds.Count > 1) return BadRequest();

            // Check to only add members who aren't a part of the project already
            var existingMembers = await (from pm in _context.ProjectMembers
                                   where pm.ProjectId == projectIds[0]
                                   select pm).ToListAsync();

            ProjectMemberComparer pmc = new ProjectMemberComparer();
            var addedMembers = projectMembers.list.Except(existingMembers, pmc).Distinct(pmc).ToList();
          
            _context.ProjectMembers.AddRange(addedMembers);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        // DELETE: api/ProjectMembers/5
        [HttpDelete("{userId}/{projectId}")]
        public async Task<ActionResult<ProjectMember>> DeleteProjectMember(uint userId, uint projectId)
        {
            var projectMember = await _context.ProjectMembers.FindAsync((userId, projectId));
            if (projectMember == null)
            {
                return NotFound();
            }

            _context.ProjectMembers.Remove(projectMember);
            await _context.SaveChangesAsync();

            return projectMember;
        }

    }
}
