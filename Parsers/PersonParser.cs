using myYSTU.Model;
using myYSTU.Utils;

namespace myYSTU.Parsers
{
    public static class PersonParser
    {
        private static INetUtils _netUtil;

        public static async Task<Person> ParseInfo()
        {
            _netUtil = DependencyService.Get<INetUtils>();

            Person student = new Person();

            var _htmlDoc = await _netUtil.GetHtmlDoc(Links.AccountInfoLink);

            //Получение ФИО
            student.Name = _htmlDoc.DocumentNode.SelectSingleNode("//h1").InnerText;

            //Получение группы
            student.Group = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[4]/td[2]").InnerText;

            //Получееие аватарки
            student.AvatarUrl = _htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[4]/div[1]/div[1]/table[1]/tr[1]/td[1]/img[1]").GetAttributeValue("src", "");

            student.Status = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[2]/td[2]").InnerText;

            student.Faculty = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[3]/td[2]").InnerText;

            student.StudyDirection = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[5]/td[2]").InnerText;

            student.SourceOfFunding  = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[1]/td[1]").InnerText;

            student.Qualification = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[2]/td[1]").InnerText;

            student.FormOfEducation = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[3]/td[1]").InnerText;

            student.DateOfBirth = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[4]/td[1]").InnerText;

            student.HomeAddress = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[5]/td[1]").InnerText;

            student.Email = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[6]/td[1]").InnerText;

            student.PhoneNumber = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[7]/td[1]").InnerText;

            Links.TimeTableLinkParams = _htmlDoc.DocumentNode.SelectSingleNode("//div[1]/div[1]/div[4]/div[1]/font[1]/table[1]/tr[1]/td[2]/font[1]/i[1]/a[1]").GetAttributeValue("href", "");

            return student;
        }

        public static async Task<ImageSource> ParseAvatar(string avatarURL)
        {
            return await _netUtil.GetImage(avatarURL);
        }
    }
}
