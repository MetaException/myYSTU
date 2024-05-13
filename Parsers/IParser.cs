using Microsoft.Datasync.Client;
using myYSTU.Models;

namespace myYSTU.Parsers
{
    public interface IParser<T> where T : IModel
    {
        public Task<List<T>> ParseInfo(string postContent = null);

        public Task<ConcurrentObservableCollection<T>> ParallelParseInfo(string postContent = null);

        public Task ParseAvatarsAsync<T>(ConcurrentObservableCollection<T> list) where T : IAvatarModel;
    }
}
