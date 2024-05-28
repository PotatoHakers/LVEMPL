using AutEmplAcc.Models;
using Microsoft.EntityFrameworkCore;

namespace AutEmplAcc.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<SickLeave> SickLeaves { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        //public DbSet<CandidateEmployee> CandidateEmployees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration for relationships, if needed
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId);

            modelBuilder.Entity<Vacation>()
                .HasOne(v => v.Employee)
                .WithMany(e => e.Vacations)
                .HasForeignKey(v => v.EmployeeId);

            modelBuilder.Entity<SickLeave>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.SickLeaves)
                .HasForeignKey(s => s.EmployeeId);

            //modelBuilder.Entity<CandidateEmployee>()
            //    .HasKey(ce => ce.Id); // Первичный ключ

            //modelBuilder.Entity<CandidateEmployee>()
            //    .HasOne(ce => ce.Candidate)
            //    .WithMany()
            //    .HasForeignKey(ce => ce.CandidateId)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Employee>()
        .HasOne(e => e.Candidate)
        .WithOne(c => c.Employee) // One-to-one relationship
        .HasForeignKey<Employee>(e => e.CandidateId);
        }
    }
}
