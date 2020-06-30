using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class Project : ProjectDTO
    {
        [Required(ErrorMessage = "Description field is required for Project")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Created By field is required for Project")]
        public uint CreatorId { get; set; }

        public uint LeaderId { get; set; }
    }
}
