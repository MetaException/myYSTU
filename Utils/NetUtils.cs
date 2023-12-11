using CommunityToolkit.Maui.Converters;
using HtmlAgilityPack;
using myYSTU.Model;
using System.Text;
#if ANDROID
    using Xamarin.Android.Net;
#endif

namespace myYSTU.Utils
{
    public static class NetUtils
    {

#if ANDROID
        private static readonly AndroidMessageHandler _handler = new AndroidMessageHandler();
#else
        private static readonly HttpClientHandler _handler = new HttpClientHandler();
#endif
        private static readonly HttpClient _client = new HttpClient(_handler) { BaseAddress = new Uri(Links.BaseUri) };

        public static async Task<bool> Authorize(string login, string password)
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

        public static async Task<HtmlDocument> PostWebData(string url, StringContent stringContent = null, MultipartFormDataContent multipartFormDataContent = null)
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

        public static async Task<byte[]> GetWebData(string url)
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

        public static async Task<HtmlDocument> GetHtmlDoc(string url)
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

        public static async Task<ImageSource> GetImage(string url)
        {
            var byteImage = await GetWebData(url);
            return new ByteArrayToImageSourceConverter().ConvertFrom(byteImage);
        }
    }
}
