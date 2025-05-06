using api;
using Microsoft.Extensions.Options;
using Octokit;
using Service;

namespace GitHub
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _option;
        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            _option = options.Value;
            _client = new GitHubClient(new ProductHeaderValue("my-github-app"))
            {
                Credentials = new Credentials(_option.GitHubToken) 
            };
        }
        public async Task<int> GetUserFollowersAsync()
        {
            var user = await _client.User.Get(_option.UserName);
            return user.Followers;
        }
        public async Task<List<Repository>> SearchRepositoriesInCSharp()
        {
            var request = new SearchRepositoriesRequest("repo-name") { Language = Language.CSharp };
            var result = await _client.Search.SearchRepo(request);
            return result.Items.ToList();
        }
        public async Task<List<RepositoryPortfolio>> GetPortfolioAsync()
        {
            var repositories = await _client.Repository.GetAllForUser(_option.UserName);
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
        public async Task<List<Repository>> SearchRepositoriesAsync(string repoName = null, string language = null)
        {
            var request = new SearchRepositoriesRequest(repoName)
            {
                Language = language != null ? (Language?)Enum.Parse(typeof(Language), language, true) : null,
                User = _option.UserName,
            };

            var result = await _client.Search.SearchRepo(request);
            return result.Items.ToList();
        }
        public async Task<DateTime> GetLastUpdateTimeAsync()
        {
            var repositories = await _client.Repository.GetAllForUser(_option.UserName);

            // אם אין ריפוזיטוריז, החזר DateTime מינימלי
            if (repositories.Count == 0)
            {
                return DateTime.MinValue;
            }

            // קבל את זמן העדכון האחרון של הריפוזיטורי הראשון
            var lastCommitDate = DateTime.MinValue;

            foreach (var repo in repositories)
            {
                var lastCommit = await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
                var lastCommitDateForRepo = lastCommit.OrderByDescending(c => c.Commit.Committer.Date).FirstOrDefault()?.Commit.Committer.Date;

                if (lastCommitDateForRepo.HasValue && lastCommitDateForRepo.Value > lastCommitDate)
                {
                    lastCommitDate = lastCommitDateForRepo.Value.DateTime; 
                }
            }

            return lastCommitDate;
        }

    }
}
