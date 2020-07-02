using issue_indexer_server.Models;
using issue_indexer_server.Models.DTO;

namespace issue_indexer_server.Controllers
{
    public class Functions
    {
        public static UserDTO UserToDTO(User user) =>
            new UserDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

        public static ProjectDTO ProjectToDTO(Project project) =>
            new ProjectDTO()
            {
                Id = project.Id,
                Name = project.Name,
                ManagerId = project.ManagerId,
                CreatedOn = project.CreatedOn,
                IsDeleted = project.IsDeleted
            };

        public static TicketDTO TicketToDTO(Ticket ticket) =>
            new TicketDTO()
            {
                Id = ticket.Id,
                Name = ticket.Name,
                CreatedOn = ticket.CreatedOn,
                Status = ticket.Status,
                AssignedTo = ticket.AssignedTo,
                SubmittedBy = ticket.SubmittedBy,
                IsDeleted = ticket.IsDeleted
            };
    }
}
