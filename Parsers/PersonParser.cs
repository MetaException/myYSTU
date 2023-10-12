using MauiApp1.Model;
using MauiApp1.Utils;

namespace MauiApp1.Parsers
{
    public static class PersonParser
    {
        public static async Task<Person> ParseInfo()
        {
            var _netUtil = DependencyService.Get<INetUtils>();

            Person student = new Person();

            var _htmlDoc = await _netUtil.getHtmlDoc("/WPROG/lk/lkstud.php", "windows-1251");

            //Получение ФИО
            student.Name = _htmlDoc.DocumentNode.SelectSingleNode("//h1").InnerText;

            //Получение группы
            student.Group = _htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[4]/div[1]/div[1]/table[1]/tr[1]/td[2]/table[1]/tr[4]/td[2]").InnerText;

            //Получееие аватарки
            string imageUrl = _htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[4]/div[1]/div[1]/table[1]/tr[1]/td[1]/img[1]").GetAttributeValue("src", "");
            student.Avatar = await _netUtil.getImage(imageUrl);

            return student;
        }
    }
}
