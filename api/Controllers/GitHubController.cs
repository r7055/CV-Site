using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public GitHubController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet("portfolio/{userName}")]
        public async Task<ActionResult<List<RepositoryPortfolio>>> GetPortfolio(string userName)
        {
            var portfolio = await _gitHubService.GetPortfolioAsync(userName);
            return Ok(portfolio);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Repository>>> SearchRepositories(
            [FromQuery] string repoName = null,
            [FromQuery] string language = null,
            [FromQuery] string userName = null)
        {
            var repositories = await _gitHubService.SearchRepositoriesAsync(repoName, language, userName);
            return Ok(repositories);
        }

        [HttpGet("followers/{userName}")]
        public async Task<ActionResult<int>> GetUserFollowers(string userName)
        {
            var followersCount = await _gitHubService.GetUserFollowersAsync(userName);
            return Ok(followersCount);
        }

        [HttpGet("search/csharp")]
        public async Task<ActionResult<List<Repository>>> SearchRepositoriesInCSharp()
        {
            var repositories = await _gitHubService.SearchRepositoriesInCSharp();
            return Ok(repositories);
        }
    }
}