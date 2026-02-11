using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace PropPropMT5ConnectionService.Helpers
{
    /// <summary>
    /// An adapter that allows Web API to use the Microsoft.Extensions.DependencyInjection container.
    /// </summary>
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDependencyScope BeginScope()
        {
            var scope = _serviceProvider.CreateScope();
            return new ServiceProviderDependencyScope(scope);
        }

        public void Dispose()
        {
            // The root service provider is not disposed by this resolver.
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }
    }

    /// <summary>
    /// A scope for the dependency resolver.
    /// </summary>
    public class ServiceProviderDependencyScope : IDependencyScope
    {
        private readonly IServiceScope _serviceScope;

        public ServiceProviderDependencyScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        public object GetService(Type serviceType)
        {
            return _serviceScope.ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceScope.ServiceProvider.GetServices(serviceType);
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
