using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models.DTO
{
    public class ProjectMemberList
    {
        [Required(ErrorMessage = "User ID field is required for Project Members")]
        public List<ProjectMember> list { get; set; }
    }
}
