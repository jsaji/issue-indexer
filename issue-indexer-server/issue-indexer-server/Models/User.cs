using issue_indexer_server.Models.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models {

    public class User : IdentityUser<uint> {
        [Required(ErrorMessage = "Joined On field is required for User")]
        public DateTime JoinedOn { get; set; }

        [Required(ErrorMessage = "First name field is required for User")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name field is required for User")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Account Type field is required for User")]
        public byte AccountType { get; set; }

    }
}
