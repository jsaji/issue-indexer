using issue_indexer_server.Models.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models {

    public class User : IdentityUser<uint> {
        public DateTime JoinedOn { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte AccountType { get; set; }

    }

    public class LoginModel {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel : LoginModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
