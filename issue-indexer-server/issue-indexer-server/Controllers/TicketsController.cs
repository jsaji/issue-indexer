using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using issue_indexer_server.Models;
using issue_indexer_server.Data;

namespace issue_indexer_server.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase {

        private readonly IssueIndexerContext _context;

        public TicketsController(IssueIndexerContext context) {
            _context = context;
        }

        // GET: api/Tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTickets(uint? projectId) {
            List<TicketDTO> tickets = null;
            if (projectId.HasValue) {
                var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
                if (!projectExists) return NotFound();

                tickets = await (from t in _context.Tickets
                                 where t.ProjectId == projectId
                                 select t as TicketDTO).ToListAsync();
            }
            if (tickets != null) return tickets;
            //else return NotFound();
            return await _context.Tickets.Select(t => (TicketDTO)t).ToListAsync();
        }

        // GET: api/Tickets/5
        [HttpGet("{ticketId}")]
        public async Task<ActionResult<Ticket>> GetTicket(uint ticketId) {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)  return NotFound();
            return ticket;
        }

        // PUT: api/Tickets/5?userId=3
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{ticketId}")]
        public async Task<IActionResult> PutTicket(uint ticketId, Ticket updatedTicket, uint? userId) {
            if (!userId.HasValue) return BadRequest();
            if (ticketId != updatedTicket.Id) return BadRequest();

            var editor = await _context.Users.FindAsync(userId);
            var originalTicket = await _context.Tickets.FindAsync(updatedTicket.Id);
            if (originalTicket == null || editor == null) return NotFound();

            try {
                // If the ticket is "deleted" or "undeleted", it applies the soft delete and does not change any other fields
                if (originalTicket.IsDeleted != updatedTicket.IsDeleted) await SoftDeleteTicket(originalTicket, updatedTicket.IsDeleted, userId.Value);
                else await EditTicketFields(originalTicket, updatedTicket, userId.Value);
                return NoContent();
            } catch (Exception) {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<IActionResult> EditTicketFields(Ticket originalTicket, Ticket updatedTicket, uint editorId) {
            originalTicket.Name = updatedTicket.Name;
            originalTicket.Description = updatedTicket.Description;
            originalTicket.AssignedTo = updatedTicket.AssignedTo;
            originalTicket.Priority = updatedTicket.Priority;
            originalTicket.Status = updatedTicket.Status;
            originalTicket.Type = updatedTicket.Type;
            originalTicket.LastModifiedOn = DateTime.UtcNow;
            originalTicket.Status = updatedTicket.Status;
            var ticketHistory = Functions.TicketToHistory(originalTicket, editorId, "edit");

            _context.TicketHistory.Add(ticketHistory);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<IActionResult> SoftDeleteTicket(Ticket originalTicket, bool softDelete, uint editorId) {
            originalTicket.IsDeleted = softDelete;
            var ticketHistory = Functions.TicketToHistory(originalTicket, editorId, "delete");
            _context.TicketHistory.Add(ticketHistory);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Tickets
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket, uint? userId) {
            if (!userId.HasValue) return BadRequest();
            // maybe check if user exists here

            ticket.IsDeleted = false;
            ticket.CreatedOn = DateTime.UtcNow;
            ticket.LastModifiedOn = DateTime.UtcNow;
            var ticketHistory = Functions.TicketToHistory(ticket, userId.Value, "create");

            _context.TicketHistory.Add(ticketHistory);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticket);
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{ticketId}")]
        public async Task<ActionResult<Ticket>> DeleteTicket(uint ticketId) {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var ticketIds = new List<uint>() { ticketId };

            // Gets history & comments associated with tickets
            var ticketHistory = await Functions.GetTicketHistory(_context, ticketIds);
            var comments = await Functions.GetTicketComments(_context, ticketIds);

            _context.Tickets.Remove(ticket);
            _context.TicketHistory.RemoveRange(ticketHistory);
            _context.Comments.RemoveRange(comments);

            await _context.SaveChangesAsync();

            return ticket;
        }

        private bool TicketExists(uint id) {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
