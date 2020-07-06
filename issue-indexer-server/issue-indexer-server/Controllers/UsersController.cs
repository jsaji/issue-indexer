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

                if (superiors.HasValue && (bool)superiors) return await GetSuperiors(user.Id);
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
            var inferiors = await (from u in _context.Users
                                   join ur in _context.UserRelationships
                                   on u.Id equals ur.UserAId
                                   where ur.UserBId == userId && ur.UserBSuperior
                                   select u as UserDTO).Distinct().ToListAsync();
            return inferiors;
        }

        private async Task<ActionResult<IEnumerable<UserDTO>>> GetSuperiors(uint userId) {
             var superiors = await (from u in _context.Users
                                   join ur in _context.UserRelationships
                                   on u.Id equals ur.UserBId
                                   where ur.UserAId == userId && ur.UserBSuperior
                                   select u as UserDTO).Distinct().ToListAsync();
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
            var relationship = await (from ur in _context.UserRelationships
                                      where ur.UserAId == user.Id && ur.UserBId == admin.Id && ur.UserBSuperior
                                      select ur).FirstOrDefaultAsync();
            // if it does not exist, create one
            if (relationship == null) {
                // Creates record to link the new manager and the admin
                var newRelationship = new UserRelationship() {
                    UserAId = user.Id,
                    UserBId = admin.Id,
                    UserBSuperior = true
                };
                _context.UserRelationships.Add(newRelationship);
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

            // Gets relationships & users where manager is superior
            var userRelationships = await (from ur in _context.UserRelationships
                                        where ur.UserBId == manager.Id && ur.UserBSuperior
                                        select ur).ToListAsync();
            var userRelationshipIds = from ur in userRelationships
                                      select ur.UserAId;
            var users = await (from u in _context.Users
                               where userRelationshipIds.Contains(u.Id)
                               select u).ToDictionaryAsync(u => u.Id, u => u.AccountType);

            // For each relationship, change them according to what would happen when manager is demoted
            userRelationships.ForEach(ur => {
                if (users.ContainsKey(ur.UserAId)) {
                    if (users[ur.UserAId] == manager.AccountType) {
                        // If they're at the same level currently, it means the ex-manager will be inferior
                        var temp = ur.UserAId;
                        ur.UserAId = ur.UserBId;
                        ur.UserBId = temp;
                        ur.UserBSuperior = true;
                    } else if (users[ur.UserAId] == manager.AccountType - 1) {
                        // If they're lower by 1 level, they are no longer the superior user
                        ur.UserBSuperior = false;
                    }
                }
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
