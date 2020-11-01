using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PhoneBookModel
{
    public partial class PhoneNumbersEntities
    {
        public void SaveChangesClientWins()
        {
            // Resolve the concurrency conflict by refreshing the
            // object context before re-saving changes.
            // Update original values from the database )
            CommitChanges(ClientWins: true);
        }
        public void SaveChangesDatabaseStoreWins()
        {
            // Resolve the concurrency conflict by refreshing the
            // object context before re-saving changes.
            CommitChanges(ClientWins: false);
        }
        private void CommitChanges(bool ClientWins = true)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    // Try to save changes, which may cause a conflict.
                    this.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    foreach (var entry in ex.Entries.
                    Where(e => e.State != EntityState.Unchanged))
                    {
                        if (ClientWins)
                        {
                            DbPropertyValues dbValues = entry.GetDatabaseValues();
                            if (dbValues != null)
                                entry.OriginalValues.SetValues(dbValues);
                            else
                            {
                                entry.State = EntityState.Added;
                            }
                        }
                        else
                            entry.Reload();
                    }
                }
            } while (saveFailed);
            //OptimisticConcurrencyException handled and changes saved
        }
    }
}