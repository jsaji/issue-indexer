using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using issue_indexer_server.Models;
using Microsoft.AspNetCore.Identity;

namespace issue_indexer_server.Data {

    public class IssueIndexerContext : IdentityDbContext<User, IdentityRole<uint>, uint> {

        public IssueIndexerContext(DbContextOptions<IssueIndexerContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //modelBuilder.Entity<User>
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProjectMember>().HasKey(pm => new { pm.UserId, pm.ProjectId });
            modelBuilder.Entity<UserRelationship>().HasKey(mm => new { mm.UserAId, mm.UserBId });
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserRelationship> UserRelationships { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TicketHistory> TicketHistory { get; set; }
    }
}
