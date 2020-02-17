using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Builder
{
    public sealed class EfCoreStorage<T, TK> :
        StorageBase<T, TK>,
        IStorage<T, TK>
        where TK : class, new()
    {
        private readonly DbContext dbContext;
        private readonly DbSet<TK> dbSet;

        public EfCoreStorage(EfCoreStorageDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            dbSet = this.dbContext.Set<TK>();
        }

        public EfCoreStorage(EfCoreStorageDbContext dbContext, string keyMap) : this(dbContext)
        {
            KeyMap = keyMap;
        }

        public async Task<(bool state, TK createdData)> Create(TK newData)
        {
            if (newData == null)
            {
                return (false, new TK());
            }

            await dbSet.AddAsync(newData).ConfigureAwait(false);

            return (await dbContext.SaveChangesAsync().ConfigureAwait(false) == 1, newData);
        }

        public async Task<IEnumerable<TK>> Read()
        {
            return await dbSet.ToListAsync().ConfigureAwait(false);
        }

        public async Task<(bool state, TK currentData)> ReadByKey(T key)
        {
            if (key == null)
            {
                return (false, new TK());
            }

            var currentData = await dbSet.FindAsync(key).ConfigureAwait(false);
            return currentData == null ? (false, new TK()) : (true, currentData);
        }

        public async Task<(bool state, TK updatedData)> Update(T key, TK updateData, string[] excludes)
        {
            if (key == null || updateData == null)
            {
                return (false, new TK());
            }
            
            var currentData = await dbSet.FindAsync(key).ConfigureAwait(false);

            if (currentData == null)
            {
                return (false, new TK());
            }

            dbSet.Update(CopyProperties(currentData, updateData, excludes));
            return (await dbContext.SaveChangesAsync().ConfigureAwait(false) == 1, currentData);
        }

        public async Task<(bool state, TK deletedData)> Delete(T key)
        {
            if (key == null)
            {
                return (false, new TK());
            }

            var currentData = await dbSet.FindAsync(key).ConfigureAwait(false);

            if (currentData == null)
            {
                return (false, new TK());
            }

            dbSet.Remove(currentData);
            return (await dbContext.SaveChangesAsync().ConfigureAwait(false) == 1, currentData);
        }
    }
}