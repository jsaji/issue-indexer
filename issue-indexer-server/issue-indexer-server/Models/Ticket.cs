using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class Ticket
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }
        [DataType(DataType.Date)]
        public DateTime ModifiedOn { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public uint SubmittedBy { get; set; }
        public uint AssignedTo { get; set; }
        public uint ProjectId { get; set; }
    }
}
