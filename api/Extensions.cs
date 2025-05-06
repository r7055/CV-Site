using api.CachedServices;
using GitHub;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Service;

namespace api
{
    public static class Extensions
    {
        public static void AddGitHubIntegration(this IServiceCollection services, Action<GitHubIntegrationOptions> configuration)
        {
            // רישום ההגדרות
            services.Configure(configuration);

            // רישום השירותים
            services.AddScoped<IMemoryCache, MemoryCache>();
            services.AddScoped<IGitHubService, GitHubService>();
            services.Decorate<IGitHubService, CachedGitHubService>();
        }
    }
}
