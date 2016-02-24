using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace Forum_ASP.NET.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Comment> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Make Blog.Url required ???
            builder.Entity<Discussion>()
                .Property(b => b.FirstComment)
                .IsRequired();
        }
    }
}
