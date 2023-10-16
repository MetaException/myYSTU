using CommunityToolkit.Maui.Converters;
using HtmlAgilityPack;
using System.Text;

namespace myYSTU.Utils
{
    public class NetUtils : INetUtils
    {
        private readonly HttpClient _client;
        private readonly HttpClientHandler _handler;

        public NetUtils()
        {
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler) { BaseAddress = new Uri("https://www.ystu.ru") };
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

                var responseContent = await loginResponse.Content.ReadAsStringAsync(); //TODO: обработать переадресацию

                //Возврат - всегда переадресация, кроме неправильного логина или пароля
                if (_handler.CookieContainer.Count == 0 || responseContent.Contains("Вы ввели неправильный логин или пароль. попробуйте еще раз"))
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
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
                    return null;
                }
            }
            catch (Exception ex)
            {
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

        public async Task<HtmlDocument> getTimeTableByWeek(string url, string enc, MultipartFormDataContent content)
        {
            try
            {
                HttpResponseMessage response = await _client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsByteArrayAsync();

                    HtmlDocument doc = new HtmlDocument();

                    // Определите кодировку HTML-страницы (например, windows-1251)
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    Encoding encoding = Encoding.GetEncoding(enc);

                    doc.LoadHtml(encoding.GetString(responseContent));

                    return doc;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
