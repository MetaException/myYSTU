using HtmlAgilityPack;
using Microsoft.Datasync.Client;
using myYSTU.Model;
using myYSTU.Utils;

namespace myYSTU.Parsers
{
    public abstract class AbstractParser<T> : IParser<T> where T : IModel
    {
        protected readonly NetUtils _netUtils = DependencyService.Get<NetUtils>();
        protected string _linkToParse;

        protected virtual HttpContent GetPostContent(string date)
        {
            return null;
        }

        protected abstract List<T> ParseHtml(HtmlDocument htmlDoc);

        public async Task<List<T>> ParseInfo(string postContent = null)
        {
            HttpContent content = null;
            if (postContent is not null)
                content = GetPostContent(postContent);

            var htmlDoc = await _netUtils.GetHtmlDoc(_linkToParse, content);

            return ParseHtml(htmlDoc);
        }

        public async Task<ConcurrentObservableCollection<T>> ParallelParseInfo(string postContent = null)
        {
            HttpContent content = null;
            if (postContent is not null)
                content = GetPostContent(postContent);

            var concurrentBag = new ConcurrentObservableCollection<T>();

            var tasks = new List<Task<HtmlDocument>>();
            for (int i = 1; i <= 5; i++)
            {
                tasks.Add(_netUtils.GetHtmlDoc($"{_linkToParse}{i}", content));
            }

            while (tasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(tasks);

                tasks.Remove(completedTask);

                var htmlDoc = await completedTask;
                var infoList = ParseHtml(htmlDoc);

                concurrentBag.AddRange(infoList);
            }
            return concurrentBag;
        }

        public Task ParseAvatarsAsync<T>(ConcurrentObservableCollection<T> list) where T : IAvatarModel
        {
            return Parallel.ForEachAsync(list, async (staffInfo, ct) =>
            {
                staffInfo.Avatar = await _netUtils.GetImage(staffInfo.AvatarUrl);
            });
        }
    }
}
