using api;
using Octokit;
using Service;

namespace GitHub
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        public GitHubService()
        {
            _client = new GitHubClient(new ProductHeaderValue("my-github-app"));
        }
        public async Task<int> GetUserFollowersAsync(string userName)
        {
            var user = await _client.User.Get(userName);
            return user.Followers;
        }
        public async Task<List<Repository>> SearchRepositoriesInCSharp()
        {
            var request = new SearchRepositoriesRequest("repo-name") { Language = Language.CSharp };
            var result = await _client.Search.SearchRepo(request);
            return result.Items.ToList();
        }
        public async Task<List<RepositoryPortfolio>> GetPortfolioAsync(string userName)
        {
            var repositories = await _client.Repository.GetAllForUser(userName);
            var portfolio = new List<RepositoryPortfolio>();

            foreach (var repo in repositories)
            {
                var lastCommit = await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
                var lastCommitDate = lastCommit.OrderByDescending(c => c.Commit.Committer.Date).FirstOrDefault()?.Commit.Committer.Date;

                var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                portfolio.Add(new RepositoryPortfolio
                {
                    Name = repo.Name,
                    Languages = await GetLanguages(repo.Owner.Login, repo.Name),
                    LastCommitDate = lastCommitDate?.DateTime,
                    Stars = repo.StargazersCount,
                    PullRequestCount = pullRequests.Count,
                    HtmlUrl = repo.HtmlUrl
                });
            }

            return portfolio;
        }

        private async Task<List<string>> GetLanguages(string owner, string repoName)
        {
            var languages = await _client.Repository.GetAllLanguages(owner, repoName);
            return languages.Select(lang => lang.Name).ToList();
        }

        public async Task<List<Repository>> SearchRepositoriesAsync(string repoName = null, string language = null, string userName = null)
        {
            var request = new SearchRepositoriesRequest(repoName)
            {
                Language = language != null ? (Language?)Enum.Parse(typeof(Language), language, true) : null,
                User = userName
            };

            var result = await _client.Search.SearchRepo(request);
            return result.Items.ToList();
        }


    }
}
