using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models {

    public class TicketHistory : Ticket {
        [Required(ErrorMessage = "Ticket ID field is required for Ticket History")]
        public uint TicketId { get; set; }

        [Required(ErrorMessage = "User ID field is required for Ticket History")]
        public uint EditorId { get; set; }

        [Required(ErrorMessage = "Action field is required for Ticket History")]
        public string Action { get; set; }
    }
}
