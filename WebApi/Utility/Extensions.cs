using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Utility
{
    public static class Extensions
    {
        public static IServiceCollection AddFileDI(this IServiceCollection services)
        {
            return services;
        }

        public static IApplicationBuilder UserFileDI(this IApplicationBuilder builder)
        {
            DI.ServiceProvider = builder.ApplicationServices;
            return builder;
        }
    }

    public static class DI
    {
        public static IServiceProvider ServiceProvider { set; get; }
    }
}
