using Medicines.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<IngestionProcess> IngestionProcesses { get; set; }
        public DbSet<AuditEntry> AuditEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Laboratory).IsRequired();
                entity.Property(e => e.ActiveIngredient).IsRequired();

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            modelBuilder.Entity<IngestionProcess>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FileType).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.StartedAt).IsRequired();
            });

            modelBuilder.Entity<AuditEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntityId).IsRequired();
                entity.Property(e => e.EntityType).IsRequired();
                entity.Property(e => e.FieldName).IsRequired();
                entity.Property(e => e.OldValue).IsRequired(false);
                entity.Property(e => e.NewValue).IsRequired(false);
                entity.Property(e => e.ChangedAt).IsRequired();
                entity.Property(e => e.ChangedBy).IsRequired();
                entity.Property(e => e.SourceFile).IsRequired();
                entity.Property(e => e.SourceFileTimestamp).IsRequired();
            });
        }

        public override int SaveChanges()
        {
            SetAuditProperties();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            SetAuditProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditProperties()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Medicine && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((Medicine)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                    ((Medicine)entityEntry.Entity).LastModifiedAt = DateTime.UtcNow;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    ((Medicine)entityEntry.Entity).LastModifiedAt = DateTime.UtcNow;
                }
                else if (entityEntry.State == EntityState.Deleted)
                {
                    entityEntry.State = EntityState.Modified;
                    ((Medicine)entityEntry.Entity).IsDeleted = true;
                }
            }
        }
    }
}
