using CommunityToolkit.Maui.Converters;
using HtmlAgilityPack;
using myYSTU.Model;
using System.Text;
#if ANDROID
    using Xamarin.Android.Net;
#endif

namespace myYSTU.Utils
{
    public class NetUtils
    {

#if ANDROID
        private readonly AndroidMessageHandler _handler;
#else
        private readonly HttpClientHandler _handler;
#endif
        private readonly HttpClient _client;

        public NetUtils()
        {

#if ANDROID
        _handler = new AndroidMessageHandler();
#else
            _handler = new HttpClientHandler();
#endif

            _client = new HttpClient(_handler) { BaseAddress = new Uri(Links.BaseUri) };
        }

        //TODO: сделать класс статическим?

        public async Task<bool> Authorize(string login, string password)
        {
            // URL для первого запроса
            string loginUrl = Links.AuthorizeLink;

            // Создаем строку с данными для первого запроса
            string loginFormData = $"login={login}&password={password}";

            // Создаем контент запроса с типом "application/x-www-form-urlencoded" для первого запроса
            var loginContent = new StringContent(loginFormData, Encoding.UTF8, "application/x-www-form-urlencoded");

            // Отправляем первый POST-запрос и получаем ответ
            var loginResponse = await _client.PostAsync(loginUrl, loginContent);

            var responseContent = await loginResponse.Content.ReadAsStringAsync(); //TODO: обработать переадресацию

            //На Windows - возврат 302, на Android - 200
            if (_handler.CookieContainer.Count == 0 || responseContent.Contains("Вы ввели неправильный логин или пароль. попробуйте еще раз"))
            {
                return false;
            }

            return true;
        }

        public async Task<HtmlDocument> PostWebData(string url, StringContent stringContent = null, MultipartFormDataContent multipartFormDataContent = null)
        {
            HttpResponseMessage response;
            if (stringContent != null)
            {
                response = await _client.PostAsync(url, stringContent);
            }
            else
                response = await _client.PostAsync(url, multipartFormDataContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsByteArrayAsync();

                HtmlDocument doc = new HtmlDocument();

                //Личный кабиент имеет кодировку: windows-1251
                if (url.ToLower().Contains("wprog"))
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    Encoding w1251_enc = Encoding.GetEncoding("windows-1251");

                    responseContent = Encoding.Convert(w1251_enc, Encoding.UTF8, responseContent);
                }


                doc.LoadHtml(Encoding.UTF8.GetString(responseContent));

                return doc;
            }
            else
            {
                // ??
                return null;
            }
        }

        public async Task<byte[]> GetWebData(string url)
        {
            var lkResponse = await _client.GetAsync(url);

            // Проверяем успешность запроса
            if (lkResponse.IsSuccessStatusCode)
            {
                return await lkResponse.Content.ReadAsByteArrayAsync();
            }
            else
            {
                //??
                return null;
            }
        }

        public async Task<HtmlDocument> GetHtmlDoc(string url)
        {
            var htmlDoc = await GetWebData(url);

            //Личный кабиент имеет кодировку: windows-1251
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
            var byteImage = await GetWebData(url);
            return new ByteArrayToImageSourceConverter().ConvertFrom(byteImage);
        }
    }
}
