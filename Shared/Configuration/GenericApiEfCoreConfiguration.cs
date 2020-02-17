using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public sealed class GenericApiEfCoreConfiguration
    {
        public HashSet<string> DbSetTypes { get; set; } = new HashSet<string>();
    }
}