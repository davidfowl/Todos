using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.Builder
{
    public sealed class GenericApiFeature :
        IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IConfiguration configuration;

        public GenericApiFeature(
            IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void PopulateFeature(
            IEnumerable<ApplicationPart> parts,
            ControllerFeature feature)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            var genericApiConfig = configuration.BindGenericApiConfiguration();

            genericApiConfig
                .Controllers
                .Select(s => Type.GetType(s.AssemblyQualifiedName))
                .Where(p => p != null && !string.IsNullOrEmpty(p.AssemblyQualifiedName))
                .ToList()
                .ForEach(controllerType =>
                {
                    genericApiConfig
                        .Controllers
                        .Where(p => p.AssemblyQualifiedName.Equals(controllerType.AssemblyQualifiedName, StringComparison.InvariantCultureIgnoreCase))
                        .SelectMany(s => s.Types)
                        .Select(s =>
                        {
                            var tk = Type.GetType(s.Type);
                            var id = Type.GetType(s.IdType);

                            return new
                            {
                                Tk = tk,
                                Id = id,
                                IsNotNullType = tk != null && id != null
                            };
                        })
                        .Where(p => p.IsNotNullType)
                        .ToList()
                        .ForEach(t => feature.Controllers.Add(controllerType.MakeGenericType(t.Tk, t.Id).GetTypeInfo()));
                });
        }
    }
}