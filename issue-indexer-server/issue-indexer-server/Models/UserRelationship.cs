using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models {
    public class UserRelationship {
 
        [Required(ErrorMessage = "User A ID field is required for Managed Members")]
        public uint UserAId { get; set; }

        [Required(ErrorMessage = "User B ID field is required for Managed Members")]
        public uint UserBId { get; set; }

        public bool UserBSuperior { get; set; }
    }
}
