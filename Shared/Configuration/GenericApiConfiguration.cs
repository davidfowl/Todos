using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public sealed class GenericApiConfiguration
    {
        public HashSet<GenericApiControllerConfiguration> Controllers { get; set; } = new HashSet<GenericApiControllerConfiguration>();
    }
}