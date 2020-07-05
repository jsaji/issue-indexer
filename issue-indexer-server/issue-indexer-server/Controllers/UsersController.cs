using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using issue_indexer_server.Models;
using issue_indexer_server.Data;
using issue_indexer_server.Models.DTO;

namespace issue_indexer_server.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {

        private readonly IssueIndexerContext _context;

        public UsersController(IssueIndexerContext context) {
            _context = context;
        }

        // GET: api/Users?userId=1&superiors=true -> returns a user's superiors
        // or api/Users?userId=1 -> returns a user's inferiors
        // or api/Users?projectId=1 -> returns users that are members of a project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(uint? userId, bool? superiors, uint? projectId) {
            if (userId.HasValue) {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                if (superiors.HasValue && (bool)superiors) return await GetSuperiors(user.Id, user.AccountType);
                else return await GetInferiors(user.Id, user.AccountType);
            } else if (projectId.HasValue) {
                bool projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
                if (!projectExists) return NotFound();

                return await GetProjectMembers(projectId.Value);
            }

            // return NotFound();
            return await _context.Users.Select(x => (UserDTO)x).ToListAsync();
        }

        private async Task<ActionResult<IEnumerable<UserDTO>>> GetProjectMembers(uint projectId) {
            List<UserDTO> users = null;

            users = await (from u in _context.Users
                           join pm in _context.ProjectMembers
                           on u.Id equals pm.UserId
                           where pm.ProjectId == projectId
                           select u as UserDTO).ToListAsync();

            if (users != null) return users;
            else return NotFound();
        }

        private async Task<ActionResult<IEnumerable<UserDTO>>> GetInferiors(uint userId, byte accountType) {
            List<UserDTO> inferiors = null;
            if (accountType == 1) {
                // Gets users that the manager has added
                inferiors = await (from u in _context.Users
                                   join mm in _context.ManagedMembers
                                   on u.Id equals mm.UserId
                                   where mm.ManagerId == userId
                                   select u as UserDTO).ToListAsync();
            } else if (accountType == 2) {
                // Gets users that the Admin has 'added'
                var users = await (from u in _context.Users
                                   join mm in _context.ManagedMembers
                                   on u.Id equals mm.UserId
                                   where mm.ManagerId == userId || mm.AdminId == userId
                                   select u as UserDTO).ToListAsync();

                // Gets managers the Admin has 'added'
                var managers = await (from u in _context.Users
                                      join mm in _context.ManagedMembers
                                      on u.Id equals mm.ManagerId
                                      where mm.AdminId == userId
                                      select u as UserDTO).ToListAsync();

                // Gets users that the Admin's managers have 'added'
                var managerids = from m in managers select m.Id;

                var misc = await (from u in _context.Users
                                  join mm in _context.ManagedMembers
                                  on u.Id equals mm.UserId
                                  where managerids.Contains(mm.ManagerId)
                                  select u as UserDTO).ToListAsync();
                inferiors = users.Concat(managers).Concat(misc).ToList();
            } else {
                // If request is made from an unacknowledged account type, nothing is retured
                return NoContent();
            }
            if (inferiors != null) inferiors = inferiors.Distinct().OrderBy(user => user.FirstName).ToList();
            return inferiors;
        }

        private async Task<ActionResult<IEnumerable<UserDTO>>> GetSuperiors(uint userId, byte accountType) {
            List<UserDTO> superiors = null;
            if (accountType == 0) {
                // If the user is a normal user, it gets associated managers and admins
                var managers = await (from u in _context.Users
                                      join mm in _context.ManagedMembers
                                      on u.Id equals mm.ManagerId
                                      where mm.UserId == userId
                                      select u as UserDTO).Distinct().ToListAsync();

                var admins = await (from u in _context.Users
                                    join mm in _context.ManagedMembers
                                    on u.Id equals mm.AdminId
                                    where mm.UserId == userId
                                    select u as UserDTO).Distinct().ToListAsync();

                HashSet<uint> managerids = new HashSet<uint>(from m in managers
                                                             select m.Id);

                var misc = await (from u in _context.Users
                                  join mm in _context.ManagedMembers
                                  on u.Id equals mm.AdminId
                                  where managerids.Contains(mm.ManagerId)
                                  select u as UserDTO).ToListAsync();

                superiors = managers.Concat(admins).Concat(misc).Distinct().OrderBy(user => user.FirstName).ToList();
            } else if (accountType == 1) {
                // If the user is a manager, it gets admins
                superiors = await (from u in _context.Users
                                   join mm in _context.ManagedMembers
                                   on u.Id equals mm.AdminId
                                   where mm.ManagerId == userId || mm.UserId == userId
                                   select u as UserDTO).ToListAsync();
            } else {
                return NoContent();
            }

            if (superiors != null) superiors = superiors.Distinct().OrderBy(user => user.FirstName).ToList();
            return superiors;
        }

        // GET: api/Users/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUser(uint userId) {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();
            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{userId}")]
        public async Task<IActionResult> PutUser(uint userId, UserDTO updatedUser, bool? promote, uint? adminId) {
            if (userId != updatedUser.Id) return BadRequest();
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            if (promote.HasValue && adminId.HasValue) {
                var admin = await _context.Users.FindAsync(adminId);
                if (admin == null || admin.AccountType < 2) return Forbid();

                if (promote.Value) return await PromoteUser(user, admin);
                else return await DemoteUser(user, admin);

            } else {
                return await EditUserDetails(user, updatedUser);
            }

        }

        private async Task<IActionResult> EditUserDetails(User user, UserDTO updatedUser) {
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            try {
                await _context.SaveChangesAsync();
            } catch (Exception) {
                throw;
            }
            return NoContent();
        }

        // Promotes a user to manager status
        private async Task<IActionResult> PromoteUser(User user, User admin) {
            // Checks to see if relation between admin and user exists
            var relationship = await (from mm in _context.ManagedMembers
                                      where mm.UserId == user.Id && mm.AdminId == admin.Id
                                      select mm).FirstOrDefaultAsync();
            // if it does not exist, create one
            if (relationship == null) {
                // Creates record to link the new manager and the admin
                var newRelationship = new ManagedMember() {
                    ManagerId = user.Id,
                    AdminId = admin.Id
                };
                _context.ManagedMembers.Add(newRelationship);
            } else {
                relationship.ManagerId = user.Id;
                relationship.UserId = 0;
            }

            // Promotes account type to manager
            user.AccountType = 1;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception) {
                throw;
            }
            return NoContent();
        }

        private async Task<IActionResult> DemoteUser(User manager, User admin) {
            // changes manager-admin relationship to user-admin
            var relationship = await (from mm in _context.ManagedMembers
                                      where mm.ManagerId == manager.Id && mm.AdminId == admin.Id
                                      select mm).FirstOrDefaultAsync();
            relationship.ManagerId = 0;
            relationship.UserId = manager.Id;


            // Gets members that were managed by the ex-manager and changes relationship
            // Members are still part of 
            var managedMembers = await (from mm in _context.ManagedMembers
                                        where mm.ManagerId == manager.Id && mm.UserId != 0
                                        select mm).ToListAsync();
            managedMembers.ForEach(mm => {
                mm.ManagerId = 0;
                mm.AdminId = admin.Id;
            });

            // Gets projects managed by ex-manager, and sets manager Id to admin
            // i.e. makes admin manager of their projects
            var managedProjects = await (from p in _context.Projects
                                         where p.ManagerId == manager.Id
                                         select p).ToListAsync();
            managedProjects.ForEach(p => p.ManagerId = admin.Id);

            // Demotes account type to standard user
            manager.AccountType = 0;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception) {
                return Conflict();
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user) {
            user.JoinedOn = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(uint id) {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(uint id) {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
