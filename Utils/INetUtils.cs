using HtmlAgilityPack;

namespace myYSTU.Utils
{
    internal interface INetUtils
    {
        public Task<int> Authorize(string login, string password);

        public Task<byte[]> GetWebData(string url);

        public Task<HtmlDocument> GetHtmlDoc(string url);

        public Task<ImageSource> GetImage(string url);

        public Task<HtmlDocument> PostWebData(string url, StringContent stringContent = null, MultipartFormDataContent multipartFormDataContent = null);
    }
}
