using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.Builder
{
    public static class GenericApiConfigurationExtensions
    {
        private const string EfCoreDbSetConfig = "EfCoreDbSet";
        private const string GenericApiConfig = "GenericApi";

        public static GenericApiConfiguration BindGenericApiConfiguration(this IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var genericApiConfig = new GenericApiConfiguration();
            configuration.GetSection(GenericApiConfig).Bind(genericApiConfig);
            return genericApiConfig;
        }

        public static GenericApiEfCoreConfiguration BindGenericEfCoreConfiguration(this IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var genericEfCoreConfig = new GenericApiEfCoreConfiguration();
            configuration.GetSection(EfCoreDbSetConfig).Bind(genericEfCoreConfig);
            return genericEfCoreConfig;
        }

        public static string[] ExcludeProperty(this IConfiguration configuration, Type controller, Type typeController)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (typeController == null)
            {
                throw new ArgumentNullException(nameof(typeController));
            }

            var sectionController = $"{GenericApiConfig}:Controllers:{controller.Namespace}.{controller.Name}:Types";
            foreach (var section in configuration
                .GetSection(sectionController)
                .GetChildren())
            {
                var typeControllerAssemblyQualifiedName = configuration.GetSection($"{sectionController}:{section.Key}:Type").Value;

                if (!typeControllerAssemblyQualifiedName.Equals(typeController.AssemblyQualifiedName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                return configuration.GetSection($"{sectionController}:{section.Key}:Exclude").Value.Split(',');
            }

            return Array.Empty<string>();
        }

        public static IEnumerable<KeyValuePair<string, string>> KeyValueDbSetAndGenericApi(this IConfiguration configuration, Type controller, params (Type api, Type identifier, string exclude)[] typeArgs)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var result = new List<KeyValuePair<string, string>>();
            result.AddRange(configuration.KeyValueDbSet(typeArgs.Select(s => s.api).ToArray()));
            result.AddRange(configuration.KeyValueGenericApi(controller, typeArgs));
            return result;
        }

        public static IEnumerable<KeyValuePair<string, string>> KeyValueDbSet(this IConfiguration configuration, params Type[] typeArgs)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return typeArgs
                .Select((s, i) => new KeyValuePair<string, string>($"{EfCoreDbSetConfig}:DbSetTypes:{i}", string.IsNullOrEmpty(s.AssemblyQualifiedName) ? string.Empty : s.AssemblyQualifiedName))
                .Where(p => !string.IsNullOrEmpty(p.Value));
        }

        public static IEnumerable<KeyValuePair<string, string>> KeyValueGenericApi(this IConfiguration configuration, Type controller, params (Type api, Type identifier, string exclude)[] typeArgs)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (controller == null || string.IsNullOrEmpty(controller.AssemblyQualifiedName))
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }
            
            var result = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>($"{GenericApiConfig}:Controllers:{controller.FullName}:AssemblyQualifiedName", controller.AssemblyQualifiedName)
            };
            
            for (var i = 0; i < typeArgs.Length; i++)
            {
                var (api, identifier, exclude) = typeArgs[i];

                if (api == null || identifier == null || string.IsNullOrEmpty(api.AssemblyQualifiedName) || string.IsNullOrEmpty(identifier.AssemblyQualifiedName))
                {
                    continue;
                }

                result.AddRange(new []
                {
                    new KeyValuePair<string, string>($"{GenericApiConfig}:Controllers:{controller.FullName}:Types:{i}:Type", api.AssemblyQualifiedName),
                    new KeyValuePair<string, string>($"{GenericApiConfig}:Controllers:{controller.FullName}:Types:{i}:IdType", identifier.AssemblyQualifiedName),
                    new KeyValuePair<string, string>($"{GenericApiConfig}:Controllers:{controller.FullName}:Types:{i}:Exclude", exclude)
                });
            }

            return result;
        }
    }
}