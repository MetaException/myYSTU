using CommunityToolkit.Maui.Converters;
using HtmlAgilityPack;
using MauiApp1.Views;
using Microsoft.Extensions.Logging;
using System.Text;
using Xamarin.Android.Net;

namespace MauiApp1.Utils
{
    public class NetUtilAndroid : INetUtils
    {
        private readonly HttpClient _client;
        private readonly AndroidMessageHandler _handler;
        private readonly ILogger _logger;

        public NetUtilAndroid()
        {
            _handler = new AndroidMessageHandler();
            _client = new HttpClient(_handler) { BaseAddress = new Uri("https://www.ystu.ru") };
            _logger = DependencyService.Get<ILogger<NetUtilAndroid>>();
        }

        public async Task<int> Authorize(string login, string password)
        {
            try
            {
                // URL для первого запроса
                string loginUrl = "/WPROG/auth1.php";

                // Создаем строку с данными для первого запроса
                string loginFormData = $"login={login}&password={password}";

                // Создаем контент запроса с типом "application/x-www-form-urlencoded" для первого запроса
                var loginContent = new StringContent(loginFormData, Encoding.UTF8, "application/x-www-form-urlencoded");

                // Отправляем первый POST-запрос и получаем ответ
                var loginResponse = await _client.PostAsync(loginUrl, loginContent);

                var responseContent = await loginResponse.Content.ReadAsStringAsync();

                //Возврат - всегда переадресация, кроме неправильного логина или пароля
                if (_handler.CookieContainer.Count == 0 || responseContent.Contains("Вы ввели неправильный логин или пароль. попробуйте еще раз"))
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return -1;
            }
            return 1;
        }

        public async Task<byte[]> GetWebData(string url)
        {
            try
            {
                var lkResponse = await _client.GetAsync(url);

                // Проверяем успешность запроса
                if (lkResponse.IsSuccessStatusCode)
                {
                    return await lkResponse.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    _logger.LogError(lkResponse.Content.ReadAsStringAsync().Result); //Нужно ли?
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                //TODO: Сделать вывод что нет интернета
                return null;
            }
        }

        public async Task<HtmlDocument> getHtmlDoc(string url, string enc)
        {
            var htmlDoc = await GetWebData(url);

            HtmlDocument doc = new HtmlDocument();

            // Определите кодировку HTML-страницы (например, windows-1251)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding(enc);

            doc.LoadHtml(encoding.GetString(htmlDoc));

            return doc;
        }

        public async Task<ImageSource> getImage(string url)
        {
            var byteImage = await GetWebData(url);
            return new ByteArrayToImageSourceConverter().ConvertFrom(byteImage);
        }
    }
}
