using api;
using Octokit;

namespace Service
{
    public interface IGitHubService
    {
        Task<List<RepositoryPortfolio>> GetPortfolioAsync();
        Task<List<Repository>> SearchRepositoriesAsync(string repoName = null, string language = null);
        Task<int> GetUserFollowersAsync();
        Task<List<Repository>> SearchRepositoriesInCSharp();
        Task<DateTime> GetLastUpdateTimeAsync();

    }

}
