using HtmlAgilityPack;
using myYSTU.Models;
using Serilog;

namespace myYSTU.Parsers;

class PersonParser(string linkToParse) : AbstractParser<Person>(linkToParse)
{
    protected override IEnumerable<Person> ParseHtml(HtmlDocument htmlDoc)
    {
        Person student = new()
        {
            Name = htmlDoc.DocumentNode.SelectSingleNode("//h1")?.InnerText,
            Group = htmlDoc.DocumentNode.SelectSingleNode("//table/tr/td[2]/table/tr[4]/td[2]")?.InnerText,
            AvatarUrl = htmlDoc.DocumentNode.SelectSingleNode("//table/tr/td/img")?.GetAttributeValue("src", ""),
            Status = htmlDoc.DocumentNode.SelectSingleNode("//table/tr/td[2]/table/tr[2]/td[2]")?.InnerText,
            Faculty = htmlDoc.DocumentNode.SelectSingleNode("//table/tr/td[2]/table/tr[3]/td[2]")?.InnerText,
            StudyDirection = htmlDoc.DocumentNode.SelectSingleNode("//table/tr/td[2]/table/tr[5]/td[2]")?.InnerText,
            SourceOfFunding = htmlDoc.DocumentNode.SelectSingleNode("//table/tbody/tr/td")?.InnerText,
            Qualification = htmlDoc.DocumentNode.SelectSingleNode("//table/tbody/tr[2]/td")?.InnerText,
            FormOfEducation = htmlDoc.DocumentNode.SelectSingleNode("//table/tbody/tr[3]/td")?.InnerText,
            DateOfBirth = htmlDoc.DocumentNode.SelectSingleNode("//table/tbody/tr[4]/td")?.InnerText,
            HomeAddress = htmlDoc.DocumentNode.SelectSingleNode("//table/tbody/tr[5]/td")?.InnerText,
            Email = htmlDoc.DocumentNode.SelectSingleNode("//table/tbody/tr[6]/td")?.InnerText,
            PhoneNumber = htmlDoc.DocumentNode.SelectSingleNode("//table/tbody/tr[7]/td")?.InnerText
        };

        student.ShortName = student.Name[..student.Name.LastIndexOf(' ')];

        Links.TimeTableLinkParams = htmlDoc.DocumentNode.SelectSingleNode("//table/tr/td[2]/font/i/a").GetAttributeValue("href", "");

        Log.Debug("[PersonParser] [ParseHtml] Parsed Person object: {@Person}", student);

        yield return student;
    }
}
