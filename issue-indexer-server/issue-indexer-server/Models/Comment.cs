using System;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models {

    public class Comment {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Message field is required for Comments")]
        public string Message { get; set; }

        [Required(ErrorMessage = "User ID field is required for Comments")]
        public uint UserId { get; set; }

        [Required(ErrorMessage = "Ticket ID field is required for Comments")]
        public uint TicketId { get; set; }

        [Required(ErrorMessage = "Edited field is required for Comments")]
        public bool Edited { get; set; }

        [Required(ErrorMessage = "Commented On field is required for Comments")]
        public DateTime CommentedOn { get; set; }
    }
}
