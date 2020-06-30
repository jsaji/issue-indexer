﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using issue_indexer_server.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace issue_indexer_server.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new IssueIndexerContext(
                serviceProvider.GetRequiredService<DbContextOptions<IssueIndexerContext>>()))
            {
                context.Users.AddRange(
                    new User
                    {
                        FirstName = "Jay",
                        LastName = "Gray",
                        Email = "jay@cool.com",
                        JoinedOn = DateTime.UtcNow,
                        AccountType = 2
                    },
                    new User
                    {
                        FirstName = "Kayt",
                        LastName = "E",
                        Email = "Kayt@cool.com",
                        JoinedOn = DateTime.UtcNow,
                        AccountType = 1
                    },
                    new User
                    {
                        FirstName = "Pete",
                        LastName = "S",
                        Email = "Pete@cool.com",
                        JoinedOn = DateTime.UtcNow,
                        AccountType = 0
                    }
                );
                context.Projects.AddRange(
                    new Project
                    {
                        Name = "Coolio 1",
                        ManagerId = 2,
                        CreatedOn = DateTime.UtcNow,
                        Description = "so cool",
                        CreatorId = 2
                    },
                    new Project
                    {
                        Name = "Coolio 23",
                        ManagerId = 2,
                        CreatedOn = DateTime.UtcNow,
                        Description = "so coo232l",
                        CreatorId = 1
                    },
                    new Project
                    {
                        Name = "personal project",
                        ManagerId = 3,
                        CreatedOn = DateTime.UtcNow,
                        Description = "2 personal",
                        CreatorId = 3
                    },
                    new Project
                    {
                        Name = "admin project",
                        ManagerId = 1,
                        CreatedOn = DateTime.UtcNow,
                        Description = "2 personal 4 u",
                        CreatorId = 1
                    }
                );
                context.ProjectMembers.AddRange(
                    new ProjectMember
                    {
                        UserId = 3,
                        ProjectId = 1
                    },
                    new ProjectMember
                    {
                        UserId = 2,
                        ProjectId = 1
                    },
                    new ProjectMember
                    {
                        UserId = 3,
                        ProjectId = 2
                    },
                    new ProjectMember
                    {
                        UserId = 2,
                        ProjectId = 2
                    },
                    new ProjectMember
                    {
                        UserId = 3,
                        ProjectId = 3
                    },
                    new ProjectMember
                    {
                        UserId = 1,
                        ProjectId = 4
                    }
                );
                context.ManagedMembers.AddRange(
                    new ManagedMember
                    {
                        UserId = 3,
                        ManagerId = 2
                    },
                    new ManagedMember
                    {
                        ManagerId = 2,
                        AdminId = 1
                    }
                );
                context.SaveChanges();

            }
        }
    }
}