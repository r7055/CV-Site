using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
  public  class RepositoryDto
    {
        public string ?Name { get; set; }
        public string Url { get; set; }
        public List<string> ?Languages { get; set; }
        public DateTime? LastCommit { get; set; }
        public int Stars { get; set; }
        public int PullRequestCount { get; set; }
    }
}
