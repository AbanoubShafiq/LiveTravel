using AirJourney_Blog.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.DAL.Context
{
    public class AppDBContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDBContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BlogCategory>()
                .HasIndex(c => c.NameEn)
                .IsUnique();

            builder.Entity<BlogCategory>()
                .HasIndex(c => c.NameEs)
                .IsUnique();

            builder.Entity<BlogPost>()
                .HasIndex(p => new { p.TitleEn, p.BlogCategoryId })
                .IsUnique();

            builder.Entity<BlogPost>()
                .HasIndex(p => new { p.TitleEs, p.BlogCategoryId })
                .IsUnique();

            builder.Entity<AppUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
            base.OnModelCreating(builder);
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<AppUser> Users { get; set; }



    }
}
