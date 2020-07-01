using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using issue_indexer_server.Models;
using SQLitePCL;
using Microsoft.AspNetCore.Http.Features;
using issue_indexer_server.Data;

namespace issue_indexer_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IssueIndexerContext _context;

        public ProjectsController(IssueIndexerContext context)
        {
            _context = context;
        }

        // GET: api/Projects?id=1&getmanaged=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects(uint? userId, bool? getManaged)
        {
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                if (user.AccountType != 0 && getManaged.HasValue && (bool) getManaged)
                {
                    return await GetByUserIdAndAccountType(user.Id, user.AccountType);
                } else
                {
                    return await GetByUserId(user.Id);
                }
            }
            // return NotFound();
            return await _context.Projects.Select(x => Functions.ProjectToDTO(x)).ToListAsync();
        }

        private async Task<ActionResult<IEnumerable<ProjectDTO>>> GetByUserIdAndAccountType(uint userId, byte accountType)
        {
            // If the accountType is 1 (i.e. manager), it gets all the projects managed by this user
            // else, if it's 2 (i.e. admin), it gets all projects managed by managers that the admin oversees
            List<ProjectDTO> projects = null;
            if (accountType == 1)
            {
                projects = await (from p in _context.Projects
                                where p.ManagerId == userId
                                select p as ProjectDTO).ToListAsync();
            }
            else if (accountType == 2)
            {
                projects = await (from p in _context.Projects
                                join mm in _context.ManagedMembers
                                on p.ManagerId equals mm.ManagerId
                                where (mm.AdminId == userId || p.ManagerId == userId)
                                select p as ProjectDTO).ToListAsync();
            }
            /*
            if (query != null && query.Count>0)
            {
                return query;
            }*/
            return projects;
            //else return NotFound();
        }

        private async Task<ActionResult<IEnumerable<ProjectDTO>>> GetByUserId(uint userId)
        {
            // Gets the projects where a user is a team member
            var projects = await (from p in _context.Projects
                           join pm in _context.ProjectMembers
                           on p.Id equals pm.ProjectId
                           where pm.UserId == userId
                           select p as ProjectDTO).ToListAsync();
            return projects;
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(uint id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(uint id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            project.CreatedOn = DateTime.UtcNow;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(uint id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return project;
        }

        private bool ProjectExists(uint id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
