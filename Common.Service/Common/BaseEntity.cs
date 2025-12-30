//csharp Common.Service\Common\BaseEntity.cs
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Library.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Service.Common
{
    public class BaseEntity : QLVN_DbContext
    {
        private bool skipDefaultChange = false;

        public BaseEntity(string connectionString) : base()
        {
            // Constructor with connection string - QLVN_DbContext has a parameterless/on-configuring implementation.
            // If you want to pass DbContextOptions, consider adding a ctor that accepts DbContextOptions<QLVN_DbContext>.
        }

        public override int SaveChanges()
        {
            if (!skipDefaultChange)
            {
                ApplyDefaultValues();
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (!skipDefaultChange)
            {
                ApplyDefaultValues();
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public int SaveSkipDefaultChange()
        {
            skipDefaultChange = true;
            int returnValue = this.SaveChanges();
            skipDefaultChange = false;

            return returnValue;
        }

        #region Private

        private void ApplyDefaultValues()
        {
            DateTime now = DateTime.Now;
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .ToList();

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    CheckNullAndUpdate(entityEntry, "RowStatus", RowStatusConstant.Active);
                    CheckNullAndUpdate(entityEntry, "CreatedAt", now);
                    CheckNullAndUpdate(entityEntry, "CreatedBy", SessionHelper.UserId);
                    CheckNullAndUpdate(entityEntry, "UpdatedAt", now);
                    CheckNullAndUpdate(entityEntry, "UpdatedBy", SessionHelper.UserId);
                }
                else
                {
                    // For modified entities, update UpdatedAt / UpdatedBy
                    CheckNullAndUpdate(entityEntry, "UpdatedAt", now);
                    CheckNullAndUpdate(entityEntry, "UpdatedBy", SessionHelper.UserId);
                }
            }
        }

        private void CheckNullAndUpdate(EntityEntry entry, string property, object value)
        {
            var propertyInfo = entry.Entity.GetType().GetProperty(property);
            if (propertyInfo == null) return;

            // keep original semantics: set value (same as EF6 version).
            // If you prefer to only set when current value is null/default, change logic accordingly.
            propertyInfo.SetValue(entry.Entity, value);
        }

        #endregion Private
    }
}