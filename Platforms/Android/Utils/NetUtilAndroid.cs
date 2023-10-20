using CommunityToolkit.Maui.Converters;
using HtmlAgilityPack;
using System.Text;
using Xamarin.Android.Net;

namespace myYSTU.Utils
{
    public class NetUtilsAndroid : INetUtils
    {
        private readonly HttpClient _client;
        private readonly AndroidMessageHandler _handler;

        public NetUtilsAndroid()
        {
            _handler = new AndroidMessageHandler();
            _client = new HttpClient(_handler) { BaseAddress = new Uri("https://www.ystu.ru") };
        }


        //TODO: пределать чтобы при первой авторизации происходило получение страницы с личным кабинетом
        //Получать encoding из запроса
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

        public async Task<HtmlDocument> PostWebData(string url, StringContent stringContent = null, MultipartFormDataContent multipartFormDataContent = null)
        {
            try
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
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
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

        public async Task<HtmlDocument> GetHtmlDoc(string url)
        {
            var htmlDoc = await GetWebData(url);

            HtmlDocument doc = new HtmlDocument();

            //Личный кабиент имеет кодировку: windows-1251
            if (url.ToLower().Contains("wprog"))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding w1251_enc = Encoding.GetEncoding("windows-1251");

                htmlDoc = Encoding.Convert(w1251_enc, Encoding.UTF8, htmlDoc);
            }

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
