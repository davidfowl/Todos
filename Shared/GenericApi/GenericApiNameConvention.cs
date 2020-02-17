using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Microsoft.AspNetCore.Builder
{
    public sealed class GenericApiNameConvention :
        IControllerModelConvention
    {
        public void Apply(
            ControllerModel controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (!controller.ControllerType.IsGenericType)
            {
                return;
            }

            controller.ControllerName = controller
                .ControllerType
                .GenericTypeArguments[0]
                .Name;
        }
    }
}