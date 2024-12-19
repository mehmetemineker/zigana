using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZigana(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
