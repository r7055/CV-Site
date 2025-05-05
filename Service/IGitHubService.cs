using api;
using Octokit;

namespace Service
{
    public interface IGitHubService
    {
        Task<List<RepositoryPortfolio>> GetPortfolioAsync(string userName);
        Task<List<Repository>> SearchRepositoriesAsync(string repoName = null, string language = null, string userName = null);
        Task<int> GetUserFollowersAsync(string userName);
        Task<List<Repository>> SearchRepositoriesInCSharp();
        Task<DateTime> GetLastUpdateTimeAsync(string userName);

    }

}
