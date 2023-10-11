using HtmlAgilityPack;

namespace MauiApp1.Utils
{
    internal interface INetUtils
    {
        public Task<int> Authorize(string login, string password);

        public Task<byte[]> GetWebData(string url);

        public Task<HtmlDocument> getHtmlDoc(string url, string enc);

        public Task<ImageSource> getImage(string url);
    }
}
