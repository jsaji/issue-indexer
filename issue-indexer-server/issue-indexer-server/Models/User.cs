using issue_indexer_server.Models.DTO;
using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class User : UserDTO
    {
        [Required(ErrorMessage = "Joined On field is required for User")]
        public DateTime JoinedOn { get; set; }

        [Required(ErrorMessage = "Account Type field is required for User")]
        public byte AccountType { get; set; }
    }
}
