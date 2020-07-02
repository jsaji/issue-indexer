using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class ProjectDTO
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Name field is required for Project")]
        public string Name { get; set; }

        public uint ManagerId { get; set; }

        [Required(ErrorMessage = "Created On field is required for Project")]
        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Is Deleted field is required for Ticket")]
        public bool IsDeleted { get; set; }
    }
}
