using CSOB_Interview_WorkLogger.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CSOB_Interview_WorkLogger.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<WorkLog> WorkLogs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Employee>()
            .HasMany(z => z.WorkLogs)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId);
    }
}

