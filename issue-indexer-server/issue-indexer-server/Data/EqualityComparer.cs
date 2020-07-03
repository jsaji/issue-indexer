using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using issue_indexer_server.Models;

namespace issue_indexer_server.Data
{
    public class ProjectMemberComparer : IEqualityComparer<ProjectMember>
    {
        public int GetHashCode(ProjectMember pm)
        {
            if (pm == null) return 0;
            return pm.UserId.GetHashCode();
        }

        public bool Equals(ProjectMember pm1, ProjectMember pm2)
        {
            if (ReferenceEquals(pm1, pm2)) return true;
            if (pm1 is null || pm2 is null) return false;
            return pm1.UserId == pm2.UserId && pm1.ProjectId == pm2.ProjectId;
        }
    }
}
