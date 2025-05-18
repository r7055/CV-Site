using Microsoft.AspNetCore.Mvc;
using Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GitHub.Service.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        // GET: api/<GitHubController>
        private readonly IGitHubService _gitHubService;

        public GitHubController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet("portfolio")]
        public async Task<IActionResult> GetPortfolio()
        {
            var data = await _gitHubService.GetPortfolioAsync();
            return Ok(data);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
 [FromQuery] string? name,
 [FromQuery] string? language,
 [FromQuery] string? user)
        {
            var data = await _gitHubService.SearchRepositoriesAsync(name, language, user);
            return Ok(data);
        }
    }
}
