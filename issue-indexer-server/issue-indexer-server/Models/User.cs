using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace issue_indexer_server.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public byte AccountType { get; set; }
        [DataType(DataType.Date)]
        public DateTime JoinedOn { get; set; }
    }
}
