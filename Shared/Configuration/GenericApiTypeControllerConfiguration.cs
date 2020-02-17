namespace Microsoft.AspNetCore.Builder
{
    public sealed class GenericApiTypeControllerConfiguration
    {
        public string Type { get; set; } = string.Empty;

        public string IdType { get; set; } = string.Empty;

        public string Exclude { get; set; } = "Id";
    }
}
