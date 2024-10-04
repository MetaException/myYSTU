using HtmlAgilityPack;
using Serilog;
using myYSTU.Models;

namespace myYSTU.Parsers;

public class StaffParser : AbstractParser<Staff>
{
    public StaffParser(string linkToParse) : base(linkToParse)
    {
    }

    public async Task<int> ParseTaskCount(string linkToParsePattern)
    {
        var _htmlDoc = await _httpService.GetHtmlDoc($"{linkToParsePattern}1");

        var staffCntStr = _htmlDoc.DocumentNode.SelectSingleNode("/html/body/section[2]/div/div/div[2]/span").InnerText;
        int staffPagesCount = (int)Math.Ceiling(double.Parse(staffCntStr[..(staffCntStr.IndexOf(' '))]) / 100d);

        Log.Debug("[StaffParser] [ParseTaskCount] Parsed task count: {@TaskCount}", staffPagesCount);
        return staffPagesCount;
    }

    protected override IEnumerable<Staff> ParseHtml(HtmlDocument htmlDoc)
    {
        var staffDiv = htmlDoc.DocumentNode.SelectNodes("//a[@class='user user--big']");

        Log.Debug("[StaffParser] [ParseHtml] Parsed {@Count} staff", staffDiv.Count); // Argument null exception

        foreach (var staff in staffDiv)
        {
            Staff staffInfo = new Staff();
            staffInfo.Name = staff.SelectSingleNode("span[2]/span[1]").InnerText.Trim();
            staffInfo.Post = staff.SelectSingleNode("span[2]/span[2]").InnerText.Trim();
            var attributeValue = staff.SelectSingleNode("span[1]").GetAttributeValue("style", "");

            string avatarUrl;

            if (attributeValue != "")
            {
                avatarUrl = attributeValue[attributeValue.IndexOf('/')..(attributeValue.Length - 2)];
                staffInfo.AvatarUrl = avatarUrl;
            }
            else
            {
                //TODO: заменить на лого ЯГТУ
                staffInfo.AvatarUrl = "/upload/resize_cache/webp/iblock/638/neqgj65a8nu4z0y81005sc62nzb4o8r3/220_220_1/zaglushka-m.webp";
            }

            //Log.Verbose("[StaffParser] [ParseHtml] Parsed StaffInfo object: {@StaffInfo}", staffInfo);

            yield return staffInfo;
        }
    }
}
