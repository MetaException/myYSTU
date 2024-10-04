using CommunityToolkit.Maui.Converters;
using HtmlAgilityPack;
using System.Text;
using Serilog;

namespace myYSTU.Services.Http;

public class HttpService : IHttpService
{
    private readonly HttpClient _client;

    public HttpService(HttpClient client)
    {
        _client = client;
    }

    public async Task<byte[]> RequestWebData(string url, HttpContent? content = null)
    {
        var response = content is null ? await _client.GetAsync(url) : await _client.PostAsync(url, content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<HtmlDocument> GetHtmlDoc(string url, HttpContent? content = null)
    {
        int attempts = 1;
        byte[]? htmlDoc = null;

        while (htmlDoc is null)
        {
            try
            {
                htmlDoc = await RequestWebData(url, content);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[HttpService] [GetHtmlDoc] [{@Method}] Attempt {@Attempt} Received error or bad status code from {@Url}; post_content: {@PostContent}", content is null ? "GET" : "POST", attempts, url, content);

                if (attempts == 5)
                {
                    throw new HttpRequestException($"Не удалось загрузить данные после 5 попыток: {url}, {content}");
                }

                attempts++;
            }
        }

        Log.Debug("[HttpService] [GetHtmlDoc] [{@Method}] Received html document from {@Url}", content is null ? "GET" : "POST", url);

        //Личный кабинет имеет кодировку: windows-1251
        if (url.ToLower().Contains("wprog"))
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding w1251_enc = Encoding.GetEncoding("windows-1251");

            htmlDoc = Encoding.Convert(w1251_enc, Encoding.UTF8, htmlDoc);
        }

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(Encoding.UTF8.GetString(htmlDoc));

        return doc;
    }

    public async Task<ImageSource> GetImage(string url)
    {
        var byteImage = await RequestWebData(url);
        Log.Debug("[HttpService] [GetImage] Received image of {@ImageSize} bytes", byteImage.Length);
        return new ByteArrayToImageSourceConverter().ConvertFrom(byteImage);
    }
}
