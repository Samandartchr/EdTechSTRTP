using Microsoft.EntityFrameworkCore;
using Domains;
using System;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {

        public DbSet<Course> Courses { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}