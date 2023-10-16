using HtmlAgilityPack;

namespace myYSTU.Utils
{
    internal interface INetUtils
    {
        public Task<int> Authorize(string login, string password);

        public Task<byte[]> GetWebData(string url);

        public Task<HtmlDocument> getHtmlDoc(string url, string enc);

        public Task<ImageSource> getImage(string url);

        public Task<HtmlDocument> getTimeTableByWeek(string url, string enc, MultipartFormDataContent content);
    }
}
