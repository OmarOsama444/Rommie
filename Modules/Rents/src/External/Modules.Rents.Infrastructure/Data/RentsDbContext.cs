using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Common.Application;
using Common.Infrastructure.Outbox;
using Common.Infrastructure.Inbox;
using Modules.Rents.Application.Abstractions;
using Modules.Rents.Domain.Abstractions;
using Modules.Rents.Domain.Entities;

namespace Modules.Rents.Infrastructure.Data;

public class RentsDbContext(DbContextOptions<RentsDbContext> options) : DbContext(options), IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    public virtual DbSet<Governorate> Governorates { get; set; } = default!;
    public virtual DbSet<Place> Places { get; set; } = default!;
    public virtual DbSet<CsvSeedHistory> CsvSeedHistories { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema.Rents);
        modelBuilder.ApplyConfigurationsFromAssembly
        (AssemblyRefrence.Assembly);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxConsumerMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxConsumerMessageConfiguration());
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            return;

        _transaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        await SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        // This tracks all changed entries and updates the timestamps
        var entries = ChangeTracker.Entries<Entity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAtUTC = DateTime.UtcNow;
                entity.UpdatedAtUTC = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAtUTC = DateTime.UtcNow;
            }
        }
    }
}
