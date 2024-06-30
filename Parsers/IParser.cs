using myYSTU.Models;
using System.Collections.Concurrent;

namespace myYSTU.Parsers;

public interface IParser<T>
{
    public Task<IEnumerable<T>> ParseInfo(string? postContent = null);
    public Task<ConcurrentBag<T>> ParallelParseInfo(string? postContent = null);
    public Task ParseAvatarsAsync<TAvatarModel>(IEnumerable<TAvatarModel> list) where TAvatarModel : IAvatarModel;
    public Task UpdateAvatarAsync(IAvatarModel model);
    public Task<ImageSource> GetAvatarAsync(IAvatarModel model);
}
