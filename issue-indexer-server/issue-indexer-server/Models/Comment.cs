using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace issue_indexer_server.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string ProjectId { get; set; }
        [DataType(DataType.Date)]
        public DateTime CommentedOn { get; set; }
    }
}
