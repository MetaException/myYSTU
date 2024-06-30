using Microsoft.Datasync.Client;
using myYSTU.Models;

namespace myYSTU.Parsers;

public interface IParser<T>
{
    public Task<IEnumerable<T>> ParseInfo(string? postContent = null);
    public Task<ConcurrentObservableCollection<T>> ParallelParseInfo(string? postContent = null);
    public Task ParseAvatarsAsync<TAvatarModel>(IEnumerable<TAvatarModel> list) where TAvatarModel : IAvatarModel;
    public Task UpdateAvatarAsync(IAvatarModel model);
    public Task<ImageSource> GetAvatarAsync(IAvatarModel model);
}
