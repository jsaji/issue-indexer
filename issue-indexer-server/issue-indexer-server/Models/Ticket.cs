using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class Ticket
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Ticket name field is required for Ticket")]
        [StringLength(50, ErrorMessage = "Ticket name cannot be longer than 50 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Ticket description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Created On field is required for Ticket")]
        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Last Modified On field is required for Ticket")]
        public DateTime LastModifiedOn { get; set; }

        [Required(ErrorMessage = "Status field is required for Ticket")]
        [StringLength(15, ErrorMessage = "Status cannot be longer than 15 characters")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Type field is required for Ticket")]
        [StringLength(25, ErrorMessage = "Type cannot be longer than 25 characters")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Priority field is required for Ticket")]
        [StringLength(10, ErrorMessage = "Priority cannot be longer than 10 characters")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Is Deleted field is required for Ticket")]
        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Submitted By field is required for Ticket")]
        public uint SubmittedBy { get; set; }

        public uint AssignedTo { get; set; }

        [Required(ErrorMessage = "Project ID field is required for Ticket")]
        public uint ProjectId { get; set; }
    }
}
