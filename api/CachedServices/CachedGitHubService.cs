using Microsoft.Extensions.Caching.Memory;
using Octokit;
using Service;

namespace api.CachedServices
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _chache;
        private readonly string UserPortfolioKey = "UserPortfolioKey";
        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _chache = memoryCache;
        }
        Task<List<RepositoryPortfolio>> IGitHubService.GetPortfolioAsync(string userName)
        {
            if (_chache.TryGetValue(UserPortfolioKey, out var protfolio))
                return (Task<List<RepositoryPortfolio>>)protfolio;

            var cacheOptions = new MemoryCacheEntryOptions().
                 SetAbsoluteExpiration(TimeSpan.FromMinutes(30)).
                 SetSlidingExpiration(TimeSpan.FromMinutes(10));

            var portfolio = _gitHubService.GetPortfolioAsync(userName);
            _chache.Set(UserPortfolioKey, portfolio, cacheOptions);
            return portfolio;

        }

        Task<int> IGitHubService.GetUserFollowersAsync(string userName)
        {
            return _gitHubService.GetUserFollowersAsync(userName);
        }

        Task<List<Repository>> IGitHubService.SearchRepositoriesAsync(string repoName, string language, string userName)
        {
            return _gitHubService.SearchRepositoriesAsync(repoName, language, userName);
        }


        Task<List<Repository>> IGitHubService.SearchRepositoriesInCSharp()
        {
            return _gitHubService.SearchRepositoriesInCSharp();
        }
    }
}
