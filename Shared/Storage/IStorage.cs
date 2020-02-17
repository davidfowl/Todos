using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public interface IStorage<in T, TK>
    {
        Task<(bool state, TK createdData)> Create(TK newData);

        Task<IEnumerable<TK>> Read();

        Task<(bool state, TK currentData)> ReadByKey(T key);

        Task<(bool state, TK updatedData)> Update(T key, TK updateData, string[] excludes);

        Task<(bool state, TK deletedData)> Delete(T key);
    }
}