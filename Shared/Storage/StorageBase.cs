using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Microsoft.AspNetCore.Builder
{
    public abstract class StorageBase<T, TK>
    {
        protected string KeyMap { get; set; } = "Id";

        protected int LastKeyInt32 => lastKeyInt32;

        protected long LastKeyInt64 => lastKeyInt64;

        protected Guid LastKeyGuid { get; set; }

        private int lastKeyInt32 = 0;

        private long lastKeyInt64 = 0;

        protected (bool state, PropertyInfo? propInfo) GetKeyProperty(TK data, string keyName = "Id")
        {
            if (data == null)
            {
                return (false, null);
            }

            var dataType = data.GetType();

            return dataType.GetProperties().Any(p => p.Name.Equals(keyName, StringComparison.InvariantCultureIgnoreCase)) 
                ? (true, dataType.GetProperty(KeyMap))
                : (false, null);
        }

        protected (T key, TK nextIdData) AutoId(TK newData)
        {
            if (newData == null)
            {
                throw new ArgumentNullException(nameof(newData));
            }

            var (state, propInfo) = GetKeyProperty(newData);

            if (!state || propInfo == null)
            {
                throw new ArgumentNullException($"The object does not have {KeyMap} property");
            }
            
            var keyValue = propInfo.GetValue(newData);
            var keyName = propInfo.PropertyType.Name;

            if (keyName.Equals(
                typeof(int).Name,
                StringComparison.InvariantCultureIgnoreCase))
            {
                Interlocked.Increment(ref lastKeyInt32);
                propInfo.SetValue(newData, lastKeyInt32);
                return ((T)((object)LastKeyInt32), newData);
            }

            if (keyName.Equals(
                typeof(long).Name,
                StringComparison.InvariantCultureIgnoreCase))
            {
                Interlocked.Increment(ref lastKeyInt64);
                propInfo.SetValue(newData, lastKeyInt64);
                return ((T)((object)LastKeyInt64), newData);
            }

            if (!keyName.Equals(
                typeof(Guid).Name,
                StringComparison.InvariantCultureIgnoreCase))
            {
                return ((T) keyValue, newData);
            }

            var currentKey = Guid.NewGuid();
            propInfo.SetValue(newData, currentKey);
            LastKeyGuid = currentKey;
            return ((T)((object)LastKeyGuid), newData);

        }

        protected TK CopyProperties(TK currentData, TK updateData, string[] excludeProperty)
        {
            if (excludeProperty.Length == 0)
            {
                excludeProperty = new[] { KeyMap };
            }

            if (currentData == null)
            {
                throw new ArgumentNullException(nameof(currentData));
            }

            if (updateData == null)
            {
                throw new ArgumentNullException(nameof(updateData));
            }

            foreach (var fromProperty in updateData
                .GetType()
                .GetProperties()
                .Where(p => !excludeProperty.Contains(p.Name)))
            {
                foreach (var toProperty in currentData.GetType().GetProperties())
                {
                    if (fromProperty.Name != toProperty.Name || fromProperty.PropertyType != toProperty.PropertyType)
                    {
                        continue;
                    }

                    toProperty.SetValue(currentData, fromProperty.GetValue(updateData));
                    break;
                }
            }

            return currentData;
        }
    }
}