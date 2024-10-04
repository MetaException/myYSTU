using HtmlAgilityPack;

namespace myYSTU.Services.Http;

public interface IHttpService
{
    public Task<byte[]> RequestWebData(string url, HttpContent? content = null);

    public Task<HtmlDocument> GetHtmlDoc(string url, HttpContent? content = null);

    public Task<ImageSource> GetImage(string url);
}
