using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class Ticket : TicketDTO
    {
        [StringLength(500, ErrorMessage = "Ticket description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Type field is required for Ticket")]
        [StringLength(25, ErrorMessage = "Type cannot be longer than 25 characters")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Priority field is required for Ticket")]
        [StringLength(10, ErrorMessage = "Priority cannot be longer than 10 characters")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Is Deleted field is required for Ticket")]
        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Project ID field is required for Ticket")]
        public uint ProjectId { get; set; }
    }
}
