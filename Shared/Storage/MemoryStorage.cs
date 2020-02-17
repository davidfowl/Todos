using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public sealed class MemoryStorage<T, TK> :
        StorageBase<T, TK>,
        IStorage<T, TK>
        where T : notnull
        where TK : new()
    {
        private readonly ConcurrentDictionary<T, TK> dictStorage = new ConcurrentDictionary<T, TK>();

        public MemoryStorage()
        {
        }

        public MemoryStorage(string keyMap) : this()
        {
            KeyMap = keyMap;
        }

        public async Task<(bool state, TK createdData)> Create(TK newData)
        {
            if (newData == null)
            {
                return (false, new TK());
            }

            var (key, nextIdData) = AutoId(newData);

            return await Create(key, nextIdData).ConfigureAwait(false);
        }

        public async Task<(bool state, TK createdData)> Create(T key, TK newData)
        {
            if (key == null || newData == null)
            {
                return (false, new TK());
            }

            var result = dictStorage.TryAdd(key, newData);
            return result ? await ReadByKey(key).ConfigureAwait(false) : (false, new TK());
        }

        public Task<IEnumerable<TK>> Read()
        {
            return Task.FromResult(dictStorage.Values.AsEnumerable());
        }

        public Task<(bool state, TK currentData)> ReadByKey(T key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var result = dictStorage.TryGetValue(key, out var getData);
            return Task.FromResult((result, getData));
        }

        public async Task<(bool state, TK updatedData)> Update(T key, TK updateData, string[] excludes)
        {
            if (key == null || updateData == null)
            {
                return (false, new TK());
            }

            var (state, currentData) = await ReadByKey(key).ConfigureAwait(false);

            if (!state)
            {
                return (false, updateData);
            }

            var result = dictStorage.TryUpdate(key, CopyProperties(currentData, updateData, excludes), currentData);
            return (result, updateData);
        }

        public Task<(bool state, TK deletedData)> Delete(T key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var result = dictStorage.TryRemove(key, out var deletedData);
            return Task.FromResult((result, deletedData));
        }
    }
}