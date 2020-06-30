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

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects(uint? id, bool? getManaged)
        {
            if (id.HasValue)
            {
                if(getManaged.HasValue && (bool) getManaged)
                {
                    return await GetByUserIdAndAccountType((uint)id);
                } else
                {
                    return await GetByUserId((uint)id);
                }
            }
            else return await _context.Projects.Select(x => ItemToDTO(x)).ToListAsync();
        }

        private async Task<ActionResult<IEnumerable<ProjectDTO>>> GetByUserIdAndAccountType(uint userId)
        {
            // Gets the user from the Db and casts as User object
            var user = await _context.Users.FindAsync(userId);

            // If the accountType is 1 (i.e. manager), it gets all the projects managed by this user
            // else, if it's 2 (i.e. admin), it gets all projects managed by managers that the admin oversees
            List<ProjectDTO> query = null;
            if (user.AccountType == 1)
            {
                query = await (from p in _context.Projects
                                where p.ManagerId == userId
                                select p as ProjectDTO).ToListAsync();
            }
            else if (user.AccountType == 2)
            {
                query = await (from p in _context.Projects
                                join mm in _context.ManagedMembers
                                on p.ManagerId equals mm.ManagerId
                                where (mm.AdminId == userId || p.ManagerId == userId)
                                select p as ProjectDTO).ToListAsync();
            }
            if (query != null && query.Count>0)
            {
                return (ActionResult<IEnumerable<ProjectDTO>>)Activator.CreateInstance(typeof(ActionResult<IEnumerable<ProjectDTO>>), query);
            }
            else return NotFound();
        }

        private async Task<ActionResult<IEnumerable<ProjectDTO>>> GetByUserId(uint userId)
        {
            // Gets the projects where a user is a team member
            var query = await (from p in _context.Projects
                           join pm in _context.ProjectMembers
                           on p.Id equals pm.ProjectId
                           where pm.UserId == userId
                           select p as ProjectDTO).ToListAsync();
            return (ActionResult<IEnumerable<ProjectDTO>>)Activator.CreateInstance(typeof(ActionResult<IEnumerable<ProjectDTO>>), query);
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

        private static ProjectDTO ItemToDTO(Project project) =>
            new ProjectDTO()
            {
                Id = project.Id,
                Name = project.Name,
                ManagerId = project.ManagerId,
                CreatedOn = project.CreatedOn
            };
    }
}
