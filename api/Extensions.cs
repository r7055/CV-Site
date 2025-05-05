using GitHub;
using Service;

namespace api
{
    public static class Extensions
    {
        public static void AddGitHubIntegration(this IServiceCollection services,Action<GitHubIntegrationOptions>configuration)
        {
            services.Configure(configuration);
            services.AddScoped<IGitHubService, GitHubService>();
        }
    }
}
