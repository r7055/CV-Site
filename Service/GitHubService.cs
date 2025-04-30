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

    }
}
