using issue_indexer_server.Models.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models {

    public class User : IdentityUser<uint> {
        public DateTime JoinedOn { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        public byte AccountType { get; set; }

    }
}
