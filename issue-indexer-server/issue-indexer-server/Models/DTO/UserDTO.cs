using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models.DTO
{
    public class UserDTO
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "First name field is required for User")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name field is required for User")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address field is required for User")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Account Type field is required for User")]
        public byte AccountType { get; set; }
    }
}
