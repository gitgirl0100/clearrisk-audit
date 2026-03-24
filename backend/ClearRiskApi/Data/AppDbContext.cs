using ClearRiskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClearRiskApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<AuditReport> AuditReports => Set<AuditReport>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuditReport>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ContractAddress)
                    .IsRequired()
                    .HasMaxLength(42);

                entity.Property(e => e.RiskTier)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ReportHash)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.TransactionHash)
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}