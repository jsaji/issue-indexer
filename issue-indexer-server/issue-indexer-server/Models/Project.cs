using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class Project
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Name field is required for Project")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description field is required for Project")]
        public string Description { get; set; }

        public uint LeaderId { get; set; }

        [Required(ErrorMessage = "Manager ID field is required for Project")]
        public uint ManagerId { get; set; }

        [Required(ErrorMessage = "Created On field is required for Project")]
        public DateTime CreatedOn { get; set; }
    }
}
