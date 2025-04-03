using Autofac;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MyAutoFac
{
    /// <summary>
    /// 控制器注入接管
    /// </summary>
    public class AutofacControllerActivator : IControllerActivator
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacControllerActivator(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public object Create(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return _lifetimeScope.Resolve(context.ActionDescriptor.ControllerTypeInfo.AsType());
        }

        public void Release(ControllerContext context, object controller)
        {
            // Optional: Handle controller release if necessary.
        }
    }
}
