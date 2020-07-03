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
using System.Net;

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
        public async Task<ActionResult> PostProjectMember(ProjectMemberList projectMembers)
        {
            var addedMembers = await DetermineMembers(projectMembers);
            if (addedMembers == null) return BadRequest();

            /*
            // This code should be shared with the MM Controller
            var addedMemberIds = (from am in addedMembers select am.UserId).ToList();
            var project = await _context.Projects.FindAsync(addedMembers[0].ProjectId);

            if (project.ManagerId != 0)
            {
                var managedMemberIds = from u in _context.Users
                                         join mm in _context.ManagedMembers
                                         on u.Id equals mm.UserId
                                         where mm.ManagerId == project.ManagerId
                                         select u.Id;
                var unmanagedMemberIds = addedMemberIds.Except(managedMemberIds).Distinct().ToList();
            }
            */

            _context.ProjectMembers.AddRange(addedMembers);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        // DELETE: api/ProjectMembers/5
        [HttpDelete]
        public async Task<ActionResult> DeleteProjectMember(ProjectMemberList projectMembers)
        {
            var addedMembers = await DetermineMembers(projectMembers, false);
            if (addedMembers == null) return BadRequest();

            _context.ProjectMembers.RemoveRange(addedMembers);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<List<ProjectMember>> DetermineMembers(ProjectMemberList projectMembers, bool addMembers=true)
        {
            // Basic check
            if (projectMembers == null || projectMembers.list.Count == 0) return null;

            // Illegal operation to remove members between different projects in a single call
            var projectIds = projectMembers.list.Select(pm => pm.ProjectId).Distinct().ToList();
            if (projectIds.Count > 1) return null;

            // Check to only add members who aren't a part of the project already
            var existingMembers = await (from pm in _context.ProjectMembers
                                         where pm.ProjectId == projectIds[0]
                                         select pm).ToListAsync();

            var pmc = new ProjectMemberComparer();
            var changedMembers = projectMembers.list;

            if (addMembers)
            {
                changedMembers = changedMembers.Except(existingMembers, pmc).Distinct(pmc).ToList();
            } else
            {
                changedMembers = changedMembers.Intersect(existingMembers, pmc).Distinct(pmc).ToList();
            }

            return changedMembers;
        }

    }
}
