﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace issue_indexer_server.Models
{
    public class ManagedMember
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public uint ManagerId { get; set; }
        public uint AdminId { get; set; }
    }
}