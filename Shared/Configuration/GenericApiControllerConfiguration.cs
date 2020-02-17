using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public sealed class GenericApiControllerConfiguration
    {
        public string AssemblyQualifiedName { get; set; } = string.Empty;

        public HashSet<GenericApiTypeControllerConfiguration> Types { get; set; } = new HashSet<GenericApiTypeControllerConfiguration>();
    }
}