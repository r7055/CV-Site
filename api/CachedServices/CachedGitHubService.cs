using Microsoft.Extensions.Caching.Memory;
using Octokit;
using Service;

namespace api.CachedServices
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _cache;

        private readonly string UserPortfolioKey = "UserPortfolioKey";
        private DateTime _lastUpdated;
        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _cache = memoryCache;
        }

        public Task<DateTime> GetLastUpdateTimeAsync(string userName)
        {
            return _gitHubService.GetLastUpdateTimeAsync(userName);
        }

        //private const string UserPortfolioKey = "UserPortfolioKey";

        public async Task<List<RepositoryPortfolio>> GetPortfolioAsync(string userName)
        {
            // בדוק אם המידע קיים ב-cache
            if (_cache.TryGetValue(UserPortfolioKey, out List<RepositoryPortfolio> portfolio))
            {
                // בדוק אם המידע מעודכן
                var lastUpdateTime = await _gitHubService.GetLastUpdateTimeAsync(userName); // פונקציה שתשיב את זמן העדכון האחרון
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
            var newPortfolio = await _gitHubService.GetPortfolioAsync(userName);

            // עדכן את זמן העדכון האחרון
            _lastUpdated = DateTime.UtcNow;

            // הגדר את המידע החדש ב-cache
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(UserPortfolioKey, newPortfolio, cacheOptions);
            return newPortfolio;
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
