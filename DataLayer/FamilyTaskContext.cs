using Core.Abstractions;
using Domain.DataModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataLayer
{
    public class FamilyTaskContext : DbContext
    {

        public FamilyTaskContext(DbContextOptions<FamilyTaskContext> options):base(options)
        {

        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.ToTable("Member");
                entity.HasMany(x => x.Tasks)
                    .WithOne(x => x.AssignedMember);
            });

            modelBuilder.Entity<Task>()
                .ToTable("Task")
                .HasOne(p => p.AssignedMember)
                .WithMany(b => b.Tasks)
                .HasForeignKey(x=>x.AssignedMemberId);
        }
    }
}