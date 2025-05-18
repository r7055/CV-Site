using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class GitHubCachingDecorator : IGitHubService
    {
        private readonly IGitHubService _inner;
        private readonly IMemoryCache _cache;
        private readonly GitHubClient _client;
        private readonly string _username;
        private const string CacheKey = "GitHubPortfolio";

        public GitHubCachingDecorator(
            IGitHubService inner,
            IMemoryCache cache,
            IOptions<GitHubOptions> options)
        {
            _inner = inner;
            _cache = cache;

            var tokenAuth = new Credentials(options.Value.Token);
            _client = new GitHubClient(new ProductHeaderValue("GitHubCaching")) { Credentials = tokenAuth };
            _username = options.Value.Username;
        }

        public async Task<IEnumerable<RepositoryDto>> GetPortfolioAsync()
        {
            // בדיקת תאריך העדכון האחרון בגיטהאב
            var remoteRepos = await _client.Repository.GetAllForUser(_username);
            var latestUpdate = remoteRepos.Max(r => r.PushedAt?.UtcDateTime ?? DateTime.MinValue);

            if (_cache.TryGetValue<(IEnumerable<RepositoryDto> Data, DateTime LastUpdate)>(CacheKey, out var cached))
            {
                if (latestUpdate <= cached.LastUpdate)
                {
                    return cached.Data; // מחזיר מה־Cache
                }

                _cache.Remove(CacheKey); // היה עדכון - מנקה
            }

            var freshData = await _inner.GetPortfolioAsync();
            _cache.Set(CacheKey, (freshData, latestUpdate), TimeSpan.FromMinutes(30));
            return freshData;
        }

        public Task<IEnumerable<RepositoryDto>> SearchRepositoriesAsync(string? name, string? language, string? user)
        {
            return _inner.SearchRepositoriesAsync(name, language, user); // בלי קאשינג
        }
    }
}

