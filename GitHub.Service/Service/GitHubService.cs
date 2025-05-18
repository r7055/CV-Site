using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly string _username;

        public GitHubService(IOptions<GitHubOptions> options)
        {
            var tokenAuth = new Credentials(options.Value.Token);
            _client = new GitHubClient(new ProductHeaderValue("GitHub.service")) { Credentials = tokenAuth };
            _username = options.Value.Username;
        }

        public async Task<IEnumerable<RepositoryDto>> GetPortfolioAsync()
        {
            var repos = await _client.Repository.GetAllForUser(_username);

            var result = new List<RepositoryDto>();

            foreach (var repo in repos)
            {
                var languages = await _client.Repository.GetAllLanguages(_username, repo.Name);
                var pulls = await _client.PullRequest.GetAllForRepository(_username, repo.Name);
                var stars = repo.StargazersCount;

                result.Add(new RepositoryDto
                {
                    Name = repo.Name,
                    Url = repo.HtmlUrl,
                    Languages = languages.Select(lang => lang.Name).ToList(), // Fixed: Use Select to extract language names
                    LastCommit = repo.PushedAt?.DateTime,
                    Stars = stars,
                    PullRequestCount = pulls.Count
                });
            }

            return result;
        }



        private Language? LanguageFromString(string language)
        {
            // מנסה להמיר את המחרוזת ל-enum של שפה
            if (Enum.TryParse<Language>(language, true, out var parsedLanguage))
            {
                return parsedLanguage;
            }
            return null; // אם השפה לא נמצאה


        }





        public async Task<IEnumerable<RepositoryDto>> SearchRepositoriesAsync(string? name, string? language, string? user)
        {
            var request = new SearchRepositoriesRequest(name ?? "")
            {
                Language = language != null ? LanguageFromString(language) : null,
                User = user
            };

            var result = await _client.Search.SearchRepo(request);
            var list = new List<RepositoryDto>();

            foreach (var repo in result.Items)
            {
                List<string> languagesList;

                try
                {
                    var languages = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                    languagesList = languages.Select(lang => lang.Name).ToList();
                }
                catch
                {
                    // אם יש שגיאה (למשל אין הרשאות או מידע), נחזיר רשימה ריקה
                    languagesList = new List<string>();
                }

                list.Add(new RepositoryDto
                {
                    Name = repo.Name,
                    Url = repo.HtmlUrl,
                    Languages = languagesList,
                    LastCommit = repo.PushedAt?.DateTime,
                    Stars = repo.StargazersCount,
                    PullRequestCount = 0 // אפשר גם להוסיף בקשה לפול-ריקווסט אם נדרש
                });
            }

            return list;
        }

    }
}