using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace issue_indexer_server.Models
{
    public class ManagedMember
    {
        public string UserId { get; set; }
        public string ManagerId { get; set; }
        public string AdminId { get; set; }
    }
}
