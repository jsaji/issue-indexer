using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class TicketDTO
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Ticket name field is required for Ticket")]
        [StringLength(50, ErrorMessage = "Ticket name cannot be longer than 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Created On field is required for Ticket")]
        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Status field is required for Ticket")]
        [StringLength(15, ErrorMessage = "Status cannot be longer than 15 characters")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Submitted By field is required for Ticket")]
        public uint SubmittedBy { get; set; }

        public uint AssignedTo { get; set; }
    }
}
