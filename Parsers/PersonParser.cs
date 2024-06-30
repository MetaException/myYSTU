using HtmlAgilityPack;
using myYSTU.Models;

namespace myYSTU.Parsers;

class PersonParser : AbstractParser<Person>
{
    public PersonParser(string linkToParse) : base(linkToParse)
    {
    }

    protected override IEnumerable<Person> ParseHtml(HtmlDocument htmlDoc)
    {
        Person student = new()
        {
            Name = htmlDoc.DocumentNode.SelectSingleNode("//h1").InnerText,
            Group = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[4]/td[2]").InnerText,
            AvatarUrl= htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[4]/div[1]/div[1]/table[1]/tr[1]/td[1]/img[1]").GetAttributeValue("src", ""),
            Status = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[2]/td[2]").InnerText,
            Faculty = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[3]/td[2]").InnerText,
            StudyDirection = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tr[1]/td[2]/table[1]/tr[5]/td[2]").InnerText,
            SourceOfFunding = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[1]/td[1]").InnerText,
            Qualification = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[2]/td[1]").InnerText,
            FormOfEducation = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[3]/td[1]").InnerText,
            DateOfBirth = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[4]/td[1]").InnerText,
            HomeAddress = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[5]/td[1]").InnerText,
            Email = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[6]/td[1]").InnerText,
            PhoneNumber = htmlDoc.DocumentNode.SelectSingleNode("//table[1]/tbody[1]/tr[7]/td[1]").InnerText
        };
        student.ShortName =  student.Name[..student.Name.LastIndexOf(' ')];

        Links.TimeTableLinkParams = htmlDoc.DocumentNode.SelectSingleNode("//div[1]/div[1]/div[4]/div[1]/font[1]/table[1]/tr[1]/td[2]/font[1]/i[1]/a[1]").GetAttributeValue("href", "");

        yield return student;
    }
}
