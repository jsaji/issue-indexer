using Microsoft.EntityFrameworkCore;

namespace issue_indexer_server.Models
{
    public class IssueIndexerContext : DbContext
    {
        public IssueIndexerContext(DbContextOptions<IssueIndexerContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ManagedMember> ManagedMembers { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TicketHistory> TicketHistory { get; set; }
    }
}
