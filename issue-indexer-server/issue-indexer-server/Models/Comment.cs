using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace issue_indexer_server.Models
{
    public class Comment
    {
        public uint Id { get; set; }
        public string Message { get; set; }
        public uint UserId { get; set; }
        public uint ProjectId { get; set; }
        [DataType(DataType.Date)]
        public DateTime CommentedOn { get; set; }
    }
}
