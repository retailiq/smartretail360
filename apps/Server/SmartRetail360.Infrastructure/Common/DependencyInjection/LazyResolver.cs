using System;
using Microsoft.Extensions.DependencyInjection;

namespace SmartRetail360.Infrastructure.Common.DependencyInjection
{
    public class LazyResolver<T> : Lazy<T> where T : notnull
    {
        public LazyResolver(IServiceProvider provider)
            : base(() => provider.GetRequiredService<T>())
        {
        }
    }
}

// To support the lazy loading of services, we create a LazyResolver class that inherits from Lazy<T>.