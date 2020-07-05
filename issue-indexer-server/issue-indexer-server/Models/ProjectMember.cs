using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models {

    public class ProjectMember {
        //public uint Id { get; set; }

        [Required(ErrorMessage = "User ID field is required for Project Members")]
        public uint UserId { get; set; }

        [Required(ErrorMessage = "Project ID field is required for Project Members")]
        public uint ProjectId { get; set; }
    }
}
