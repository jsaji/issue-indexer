﻿using System;
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
using System.Data;

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
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects(uint? userId, bool? getManaged, bool? getDeleted)
        {
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                if (!getDeleted.HasValue) getDeleted = false;

                if (user.AccountType != 0 && getManaged.HasValue && getManaged.Value)
                {
                    return await GetByUserIdAndAccountType(user.Id, user.AccountType, getDeleted.Value);
                } else
                {
                    return await GetByUserId(user.Id, getDeleted.Value);
                }
            }
            // return NotFound();
            return await _context.Projects.Select(x => Functions.ProjectToDTO(x)).ToListAsync();
        }

        private async Task<ActionResult<IEnumerable<ProjectDTO>>> GetByUserIdAndAccountType(uint userId, byte accountType, bool getDeleted)
        {
            // If the accountType is 1 (i.e. manager), it gets all the projects managed by this user
            // else, if it's 2 (i.e. admin), it gets all projects managed by managers that the admin oversees
            List<ProjectDTO> projects = null;
            if (accountType == 1)
            {
                projects = await (from p in _context.Projects
                                where p.ManagerId == userId && p.IsDeleted == getDeleted
                                select p as ProjectDTO).ToListAsync();
            }
            else if (accountType == 2)
            {
                projects = await (from p in _context.Projects
                                join mm in _context.ManagedMembers
                                on p.ManagerId equals mm.ManagerId
                                where (mm.AdminId == userId || p.ManagerId == userId) && p.IsDeleted == getDeleted
                                select p as ProjectDTO).ToListAsync();
            }
            return projects;
            //else return NotFound();
        }

        private async Task<ActionResult<IEnumerable<ProjectDTO>>> GetByUserId(uint userId, bool getDeleted)
        {
            // Gets the projects where a user is a team member
            var projects = await (from p in _context.Projects
                           join pm in _context.ProjectMembers
                           on p.Id equals pm.ProjectId
                           where pm.UserId == userId && p.IsDeleted == getDeleted
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
        [HttpPut("{projectId}")]
        public async Task<IActionResult> PutProject(uint projectId, Project project, uint? userId)
        {
            if (!userId.HasValue) return BadRequest();
            if (projectId != project.Id) return BadRequest();

            User user = await _context.Users.FindAsync(userId);
            Project original = await _context.Projects.FindAsync(project.Id);

            if (original == null || user == null) return NotFound();
            // Changes are unauthorized if the user is not an admin, or if they're not the project manager
            // or if they're not the creator (provided there is no manager)
            if (user.AccountType != 2 && (original.ManagerId != user.Id
                || (original.ManagerId == 0 && original.CreatorId != user.Id))) return Unauthorized();

            try
            {
                // If the project is "deleted" or "undeleted", it applies the soft delete and does not change any other fields
                if (project.IsDeleted != original.IsDeleted) await SoftDeleteProject(original, project.IsDeleted);
                else if (original.ManagerId != project.ManagerId) await ChangeProjectManager(original, project.ManagerId);
                else EditProjectFields(original, project);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            
        }

        private async Task<IActionResult> ChangeProjectManager(Project original, uint managerId)
        {
            
            if (managerId != 0)
            {
                User manager = await _context.Users.FindAsync(managerId);
                if (manager == null || manager.AccountType < 1) return BadRequest();

                // Gets all current project members
                HashSet<uint> projectMembers = new HashSet<uint>
                    (await (from pm in _context.ProjectMembers     
                            where pm.ProjectId == original.Id       
                            select pm.UserId).ToListAsync());

                // If the individual made manager of the project is a member, add them
                if (!projectMembers.Contains(manager.Id))
                {
                    ProjectMember newMember = new ProjectMember()
                    {
                        ProjectId = original.Id,
                        UserId = managerId
                    };
                    _context.ProjectMembers.Add(newMember);
                }

                // Get all the project members that are not currently 'managed' by the new manager
                var unmanagedUsers = await (from mm in _context.ManagedMembers
                                 where projectMembers.Contains(mm.UserId) && (mm.ManagerId != manager.Id)
                                 select mm.UserId).ToListAsync();

                // If there are any, add records indicating that they are managed by the new manager
                if (unmanagedUsers != null && unmanagedUsers.Count > 0)
                {
                    List<ManagedMember> newMembers = new List<ManagedMember>();
                    unmanagedUsers.ForEach(user => newMembers.Add(
                        new ManagedMember()
                        {
                            UserId = user,
                            ManagerId = manager.Id
                        }
                    ));

                    _context.ManagedMembers.AddRange(newMembers);
                }

            }

            original.ManagerId = managerId;
            _context.Entry(original).State = EntityState.Modified;

            return NoContent();
        }

        private IActionResult EditProjectFields(Project original, Project project)
        {
            original.Name = project.Name;
            original.Description = project.Description;
            original.LeaderId = project.LeaderId;

            _context.Entry(original).State = EntityState.Modified;

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

        // DELETE: api/Projects/5 -> hard delete
        [HttpDelete("{projectId}")]
        public async Task<ActionResult<Project>> DeleteProject(uint projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            // Gets tickets associated with project and their individual Ids
            var tickets = await (from t in _context.Tickets
                                 where t.ProjectId == project.Id
                                 select t).ToListAsync();
            HashSet<uint> ticketIds = new HashSet<uint>(from t in tickets select t.Id);

            // Gets history & comments associated with project's tickets
            var ticketHistory = await (from th in _context.TicketHistory
                                       where ticketIds.Contains(th.TicketId)
                                       select th).ToListAsync();
            var comments = await (from c in _context.Comments
                                  where ticketIds.Contains(c.TicketId)
                                  select c).ToListAsync();

            // Gets members of project
            var projectMembers = await (from pm in _context.ProjectMembers
                                        where pm.ProjectId == project.Id
                                        select pm).ToListAsync();

            // Removes the instances from the Db and saves
            _context.Projects.Remove(project);
            _context.Tickets.RemoveRange(tickets);
            _context.TicketHistory.RemoveRange(ticketHistory);
            _context.Comments.RemoveRange(comments);
            _context.ProjectMembers.RemoveRange(projectMembers);

            try
            {
                await _context.SaveChangesAsync();
                return project;
            }
            catch (DBConcurrencyException)
            {
                return Conflict();
            }
        }

        private async Task<IActionResult> SoftDeleteProject(Project project, bool softDelete)
        {
            project.IsDeleted = softDelete;

            var tickets = await (from t in _context.Tickets
                                 where t.ProjectId == project.Id
                                 select t).ToListAsync();

            tickets.ForEach(ticket => ticket.IsDeleted = softDelete);

            _context.Entry(project).State = EntityState.Modified;
            //_context.Entry(tickets).State = EntityState.Modified;

            return NoContent();
        }

        private bool ProjectExists(uint id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
