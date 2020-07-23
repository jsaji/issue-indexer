using issue_indexer_server.Data;
using issue_indexer_server.Models;
using issue_indexer_server.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace issue_indexer_server.Controllers {

    public class Functions {

        public static TicketHistory TicketToHistory(Ticket ticket, uint editorId, string action) {
            return new TicketHistory() {
                Name = ticket.Name,
                CreatedOn = ticket.CreatedOn,
                Status = ticket.Status,
                AssignedTo = ticket.AssignedTo,
                SubmittedBy = ticket.SubmittedBy,
                IsDeleted = ticket.IsDeleted,
                Description = ticket.Description,
                Type = ticket.Type,
                Priority = ticket.Priority,
                LastModifiedOn = ticket.LastModifiedOn,
                ProjectId = ticket.ProjectId,
                TicketId = ticket.Id,
                EditorId = editorId,
                Action = action
            };
        }

        public static UserDTO UserToDTO(User user) {
            return new UserDTO() {
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccountType = user.AccountType,
                Email = user.Email,
                Id = user.Id
            };
        }

        public static async Task<List<TicketHistory>> GetTicketHistory(IssueIndexerContext _context, List<uint> ticketIds) {
            var ticketHistory = await (from th in _context.TicketHistory
                                       where ticketIds.Contains(th.TicketId)
                                       select th).AsNoTracking().ToListAsync();
            return ticketHistory;
        }

        public static async Task<List<Comment>> GetTicketComments(IssueIndexerContext _context, List<uint> ticketIds) {
            var comments = await (from c in _context.Comments
                                  where ticketIds.Contains(c.TicketId)
                                  select c).AsNoTracking().ToListAsync();
            return comments;
        }

        public static async Task<bool> UserExists(IssueIndexerContext _context, uint userId) {
            return await _context.Users.AnyAsync(e => e.Id == userId);
        }
    }
}
