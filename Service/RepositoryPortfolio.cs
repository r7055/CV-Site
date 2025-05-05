namespace api
{
    public class RepositoryPortfolio
    {
        public string Name { get; set; }
        public List<string> Languages { get; set; }
        public DateTime? LastCommitDate { get; set; }
        public int Stars { get; set; }
        public int PullRequestCount { get; set; }
        public string HtmlUrl { get; set; }
    }

}
