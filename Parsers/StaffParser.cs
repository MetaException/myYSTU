using HtmlAgilityPack;
using MauiApp1.Model;
using MauiApp1.Utils;

namespace MauiApp1.Parsers
{
    public static class StaffParser
    {
        public static async IAsyncEnumerable<Staff.StaffInfo> ParseInfo()
        {
            var _netUtil = DependencyService.Get<INetUtils>();

            int pageNumber = 1;
            int staffPagesCount = 2;

            do
            {
                var _htmlDoc = await _netUtil.getHtmlDoc($"/users/?PAGEN_1={pageNumber}", "utf-8");

                if (pageNumber == 1)
                {
                    var staffCntStr = _htmlDoc.DocumentNode.SelectSingleNode("/html/body/section[2]/div/div/div[2]/span").InnerText;
                    staffPagesCount = (int)Math.Ceiling(double.Parse(staffCntStr[..(staffCntStr.IndexOf(' '))]) / 100d);
                }
                var staffDiv = _htmlDoc.DocumentNode.SelectNodes("//a[@class='user user--big']");

                for (int i = 0; i < staffDiv.Count; i++)
                {
                    Staff.StaffInfo staffInfo = new Staff.StaffInfo();
                    staffInfo.Name = staffDiv[i].SelectSingleNode("span[2]/span[1]").InnerText.Trim();
                    staffInfo.Post = staffDiv[i].SelectSingleNode("span[2]/span[2]").InnerText.Trim();
                    var attributeValue = staffDiv[i].SelectSingleNode("span[1]").GetAttributeValue("style", "");

                    string avatarUrl = "";

                    if (attributeValue != "")
                    {
                        avatarUrl = attributeValue[attributeValue.IndexOf('/')..(attributeValue.Length - 2)];
                        staffInfo.Avatar = await _netUtil.getImage(avatarUrl);
                    }

                    yield return staffInfo;
                }

                pageNumber++;
            } while (pageNumber <= staffPagesCount);
        }
    }
}
