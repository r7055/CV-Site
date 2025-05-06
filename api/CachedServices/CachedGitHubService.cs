using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Octokit;
using Service;

namespace api.CachedServices
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _cache;
        private readonly GitHubIntegrationOptions _option;
        private readonly string UserPortfolioKey = "UserPortfolioKey";
        private DateTime _lastUpdated;

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache, IOptions<GitHubIntegrationOptions> options)
        {
            _gitHubService = gitHubService;
            _cache = memoryCache;
            _option = options.Value;
        }

        public Task<DateTime> GetLastUpdateTimeAsync()
        {
            return _gitHubService.GetLastUpdateTimeAsync();
        }

        public async Task<List<RepositoryPortfolio>> GetPortfolioAsync()
        {
            var res = _cache.Get(UserPortfolioKey);
            // בדוק אם המידע קיים ב-cache
            if (_cache.TryGetValue(UserPortfolioKey, out List<RepositoryPortfolio> portfolio))
            {
                // בדוק אם המידע מעודכן
                var lastUpdateTime = await _gitHubService.GetLastUpdateTimeAsync();
                if (lastUpdateTime > _lastUpdated)
                {
                    // אם יש עדכון, נקי את ה-cache
                    _cache.Remove(UserPortfolioKey);
                }
                else
                {
                    // אם אין עדכון, החזר את המידע מה-cache
                    return portfolio;
                }
            }

            // אם אין מידע ב-cache או אם המידע לא מעודכן, קבל מידע חדש
            var newPortfolio = await _gitHubService.GetPortfolioAsync();


            // הגדר את המידע החדש ב-cache
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(200))
                .SetSlidingExpiration(TimeSpan.FromMinutes(200));

            _cache.Set(UserPortfolioKey, newPortfolio, cacheOptions);

            // עדכן את זמן העדכון האחרון
            _lastUpdated = DateTime.UtcNow;

            return newPortfolio;
        }


        Task<int> IGitHubService.GetUserFollowersAsync()
        {
            return _gitHubService.GetUserFollowersAsync();
        }

        Task<List<Repository>> IGitHubService.SearchRepositoriesAsync(string repoName, string language)
        {
            return _gitHubService.SearchRepositoriesAsync(repoName, language);
        }


        Task<List<Repository>> IGitHubService.SearchRepositoriesInCSharp()
        {
            return _gitHubService.SearchRepositoriesInCSharp();
        }
    }
}
