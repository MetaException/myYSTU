using myYSTU.Model;
using myYSTU.Utils;

namespace myYSTU.Parsers
{
    public static class PersonParser
    {
        public static async Task<Person> ParseInfo()
        {
            var _netUtil = DependencyService.Get<INetUtils>();

            Person student = new Person();

            var _htmlDoc = await _netUtil.GetHtmlDoc("/WPROG/lk/lkstud.php");

            //Получение ФИО
            student.Name = _htmlDoc.DocumentNode.SelectSingleNode("//h1").InnerText;

            //Получение группы
            student.Group = _htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[4]/td[2]").InnerText;

            //Получееие аватарки
            string imageUrl = _htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[4]/div[1]/div[1]/table[1]/tr[1]/td[1]/img[1]").GetAttributeValue("src", "");
            student.Avatar = await _netUtil.GetImage(imageUrl);

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

            return student;
        }
    }
}
