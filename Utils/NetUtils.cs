using CommunityToolkit.Maui.Converters;
using HtmlAgilityPack;
using myYSTU.Model;
using myYSTU.Views;
using System.Text;

namespace myYSTU.Utils
{
    public class NetUtils
    {
        private readonly HttpClientHandler _handler;
        private readonly HttpClient _client;

        public NetUtils(HttpClientHandler handler, HttpClient client)
        {
            _handler = handler;
            _client = client;
        }

        public async Task<byte[]> RequestWebData(string url, HttpContent content = null)
        {
            var response = content is null ? await _client.GetAsync(url) : await _client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Redirect)
            {
                //Если не авторизован то 302
                if (!await UseSavedCredentials())
                {
                    //await App.Current.MainPage.DisplayAlert("Ошибка авторизации", "Возможно был изменён пароль учётной записи", "Авторизоваться");
                    App.Current.MainPage = new NavigationPage(new AuthPage());
                    return null;
                }

                return await RequestWebData(url, content);
            }
            return null;
        }

        public async Task<HtmlDocument> GetHtmlDoc(string url, HttpContent content = null)
        {
            byte[] htmlDoc = await RequestWebData(url, content);

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
            var byteImage = await RequestWebData(url);
            return new ByteArrayToImageSourceConverter().ConvertFrom(byteImage);
        }

        public async Task UseSavedSession()
        {
            //SecureStorage.Default.Remove("session_name");
            var session_name = await SecureStorage.Default.GetAsync("session_name");
            var session_value = await SecureStorage.Default.GetAsync("session_value");

            if (session_name is null || session_value is null)
            {
                return;
            }

            _handler.CookieContainer.SetCookies(_client.BaseAddress, $"{session_name}={session_value}");
        }

        public async Task<bool> UseSavedCredentials()
        {
            //SecureStorage.Default.Remove("login");
            var login = await SecureStorage.Default.GetAsync("login");
            var password = await SecureStorage.Default.GetAsync("password");

            if (login is null || password is null)
            {
                return false;
            }

            return await AuthorizeWithPassword(login, password);
        }

        public async Task<bool> AuthorizeWithPassword(string login, string password)
        {
            // URL для первого запроса
            string loginUrl = Links.AuthorizeLink;

            // Создаем строку с данными для первого запроса
            string loginFormData = $"login={login}&password={password}";

            // Создаем контент запроса с типом "application/x-www-form-urlencoded" для первого запроса
            var loginContent = new StringContent(loginFormData, Encoding.UTF8, "application/x-www-form-urlencoded");

            // Отправляем первый POST-запрос и получаем ответ
            var loginResponse = await _client.PostAsync(loginUrl, loginContent);

            //Неверный логин или пароль - возврат 200
            if (_handler.CookieContainer.Count == 0 || loginResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            await SecureStorage.Default.SetAsync("login", login);
            await SecureStorage.Default.SetAsync("password", password);
            await SecureStorage.Default.SetAsync("session_name", _handler.CookieContainer.GetCookies(_client.BaseAddress)[0].Name);
            await SecureStorage.Default.SetAsync("session_value", _handler.CookieContainer.GetCookies(_client.BaseAddress)[0].Value);
            return true;
        }
    }
}
