using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
   public interface IGitHubService
    {
        Task<IEnumerable<RepositoryDto>> GetPortfolioAsync();
        Task<IEnumerable<RepositoryDto>> SearchRepositoriesAsync(string? name, string? language, string? user);
    }
}
