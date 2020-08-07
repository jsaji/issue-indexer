using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using issue_indexer_server.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace issue_indexer_server.Data {

    public class SeedData {

        public static void Initialize(IServiceProvider serviceProvider) {
            var context = new IssueIndexerContext(
                serviceProvider.GetRequiredService<DbContextOptions<IssueIndexerContext>>());
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            using (userManager)
            using (context) {
                
                userManager.CreateAsync(new User {
                    UserName = "jay@cool.com".ToUpper(),
                    FirstName = "Jay",
                    LastName = "Gray",
                    Email = "jay@cool.com",
                    JoinedOn = DateTime.UtcNow,
                    AccountType = 2
                }, "Password1!"
                ).GetAwaiter().GetResult();
                userManager.CreateAsync(new User {
                    UserName = "Kayt@cool.com".ToUpper(),
                    FirstName = "Kayt",
                    LastName = "E",
                    Email = "Kayt@cool.com",
                    JoinedOn = DateTime.UtcNow,
                    AccountType = 1
                }, "Password1!"
                ).GetAwaiter().GetResult();
                userManager.CreateAsync(new User {
                    UserName = "Pete@cool.com".ToUpper(),
                    FirstName = "Pete",
                    LastName = "S",
                    Email = "Pete@cool.com",
                    JoinedOn = DateTime.UtcNow,
                    AccountType = 0
                }, "Password1!"
                ).GetAwaiter().GetResult();


                context.Projects.AddRange(
                    new Project {
                        Name = "Coolio 1",
                        ManagerId = 2,
                        CreatedOn = DateTime.UtcNow,
                        Description = "so cool",
                        CreatorId = 2
                    },
                    new Project {
                        Name = "Coolio 23",
                        ManagerId = 2,
                        CreatedOn = DateTime.UtcNow,
                        Description = "so coo232l",
                        CreatorId = 1
                    },
                    new Project {
                        Name = "personal project",
                        ManagerId = 0,
                        CreatedOn = DateTime.UtcNow,
                        Description = "2 personal",
                        CreatorId = 3
                    },
                    new Project {
                        Name = "admin project",
                        ManagerId = 1,
                        CreatedOn = DateTime.UtcNow,
                        Description = "2 personal 4 u",
                        CreatorId = 1
                    }
                );
                context.ProjectMembers.AddRange(
                    new ProjectMember {
                        UserId = 3,
                        ProjectId = 1
                    },
                    new ProjectMember {
                        UserId = 2,
                        ProjectId = 1
                    },
                    new ProjectMember {
                        UserId = 3,
                        ProjectId = 2
                    },
                    new ProjectMember {
                        UserId = 2,
                        ProjectId = 2
                    },
                    new ProjectMember {
                        UserId = 3,
                        ProjectId = 3
                    },
                    new ProjectMember {
                        UserId = 1,
                        ProjectId = 4
                    }
                );
                context.UserRelationships.AddRange(
                    new UserRelationship {
                        UserAId = 3,
                        UserBId = 2,
                        UserBSuperior = true
                    },
                    new UserRelationship {
                        UserAId = 2,
                        UserBId = 1,
                        UserBSuperior = true
                    }
                );
                context.Tickets.AddRange(
                    new Ticket {
                        Name = "Major bug lol",
                        Description = "Yyeee",
                        Status = "Open",
                        SubmittedBy = 3,
                        Type = "Bug fix",
                        Priority = "High",
                        CreatedOn = DateTime.UtcNow,
                        LastModifiedOn = DateTime.UtcNow,
                        ProjectId = 1
                    },
                    new Ticket {
                        Name = "Major bug3 bad",
                        Description = "v bad",
                        Status = "WORK DAMMIT",
                        SubmittedBy = 3,
                        Type = "Bug fix",
                        Priority = "Medium",
                        CreatedOn = DateTime.UtcNow,
                        LastModifiedOn = DateTime.UtcNow,
                        ProjectId = 1
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
