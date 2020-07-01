using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using issue_indexer_server.Models;
using issue_indexer_server.Data;
using issue_indexer_server.Models.DTO;

namespace issue_indexer_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IssueIndexerContext _context;

        public UsersController(IssueIndexerContext context)
        {
            _context = context;
        }

        // GET: api/Users?
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(uint? userId, bool? superiors)
        {
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                if (superiors.HasValue && (bool)superiors) return await GetSuperiors(user);
                else return await GetInferiors(user);
            }
            
            // return NotFound();
            return await _context.Users.Select(x => Functions.UserToDTO(x)).ToListAsync();
        }

        private async Task<ActionResult<IEnumerable<UserDTO>>> GetInferiors(User user)
        {
            List<UserDTO> inferiors = null;
            if (user.AccountType == 1)
            {
                // Gets users that the manager has added
                inferiors = await (from u in _context.Users
                               join mm in _context.ManagedMembers
                               on u.Id equals mm.UserId
                               where mm.ManagerId == user.Id
                               select u as UserDTO).ToListAsync();
            }
            else if (user.AccountType == 2)
            {
                // Gets users that the Admin has 'added'
                var users = await (from u in _context.Users
                               join mm in _context.ManagedMembers
                               on u.Id equals mm.UserId
                               where mm.ManagerId == user.Id || mm.AdminId == user.Id
                               select u as UserDTO).ToListAsync();

                // Gets managers the Admin has 'added'
                var managers = await (from u in _context.Users
                                      join mm in _context.ManagedMembers
                                      on u.Id equals mm.ManagerId
                                      where mm.AdminId == user.Id
                                      select u as UserDTO).ToListAsync();

                // Gets users that the Admin's managers have 'added'
                HashSet<uint> managerids = new HashSet<uint> (from m in managers
                                                select m.Id);

                var misc = await (from u in _context.Users
                                  join mm in _context.ManagedMembers
                                  on u.Id equals mm.UserId
                                  where managerids.Contains(mm.ManagerId)
                                  select u as UserDTO).ToListAsync();
                inferiors = users.Concat(managers).Concat(misc).ToList();
            } else
            {
                // If request is made from an unacknowledged account type, nothing is retured
                return NoContent();
            }
            if (inferiors != null) inferiors = inferiors.Distinct().OrderBy(user => user.FirstName).ToList();
            return inferiors;
        }

        private async Task<ActionResult<IEnumerable<UserDTO>>> GetSuperiors(User user)
        {
            List<UserDTO> superiors = null;
            if (user.AccountType == 0)
            {
                // If the user is a normal user, it gets associated managers and admins
                var managers = await (from u in _context.Users
                                      join mm in _context.ManagedMembers
                                      on u.Id equals mm.ManagerId
                                      where mm.UserId == user.Id
                                      select u as UserDTO).Distinct().ToListAsync();

                var admins = await (from u in _context.Users
                                    join mm in _context.ManagedMembers
                                    on u.Id equals mm.AdminId
                                    where mm.UserId == user.Id
                                    select u as UserDTO).Distinct().ToListAsync();

                HashSet<uint> managerids = new HashSet<uint>(from m in managers
                                                             select m.Id);

                var misc = await (from u in _context.Users
                                  join mm in _context.ManagedMembers
                                  on u.Id equals mm.AdminId
                                  where managerids.Contains(mm.ManagerId)
                                  select u as UserDTO).ToListAsync();

                superiors = managers.Concat(admins).Concat(misc).Distinct().OrderBy(user => user.FirstName).ToList();
            } else if (user.AccountType == 1)
            {
                // If the user is a manager, it gets admins
                superiors = await (from u in _context.Users
                                    join mm in _context.ManagedMembers
                                    on u.Id equals mm.AdminId
                                    where mm.ManagerId == user.Id || mm.UserId == user.Id
                                    select u as UserDTO).ToListAsync();
            } else
            {
                return NoContent();
            }
            if (superiors != null) superiors = superiors.Distinct().OrderBy(user => user.FirstName).ToList();
            return superiors;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(uint id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(uint id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.JoinedOn = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(uint id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(uint id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        
    }
}
