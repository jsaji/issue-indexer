using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace issue_indexer_server.Models
{
    public class Project
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint LeaderId { get; set; }
        public uint ManagerId { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }
    }
}
