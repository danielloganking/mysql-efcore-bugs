using System;
using Microsoft.EntityFrameworkCore;

namespace App
{
    public class ExampleDbContext : DbContext
    {
        public DbSet<Entity> Entities { get; set; } = null!; // just in case nullable reference types is enabled

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>(entity =>
            {
                entity.Property(e => e.Example)
                    .ValueGeneratedOnAddOrUpdate();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("<YOUR_CONNECTION_STRING_HERE>");
        }
    }
}
