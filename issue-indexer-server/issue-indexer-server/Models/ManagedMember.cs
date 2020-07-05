using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models {
    public class ManagedMember {
        public uint Id { get; set; }

        [Required(ErrorMessage = "User ID field is required for Managed Members")]
        public uint UserId { get; set; }

        public uint ManagerId { get; set; }

        public uint AdminId { get; set; }
    }
}
