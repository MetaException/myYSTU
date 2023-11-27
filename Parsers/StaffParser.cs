﻿using myYSTU.Model;
using myYSTU.Utils;

namespace myYSTU.Parsers
{
    public static class StaffParser
    {
        public static async IAsyncEnumerable<Staff> ParseInfo()
        {
            var _netUtil = DependencyService.Get<INetUtils>();

            int pageNumber = 1;
            int staffPagesCount = 2;

            do
            {
                var _htmlDoc = await _netUtil.GetHtmlDoc($"{Links.StaffLink}{pageNumber}");

                if (pageNumber == 1)
                {
                    var staffCntStr = _htmlDoc.DocumentNode.SelectSingleNode("/html/body/section[2]/div/div/div[2]/span").InnerText;
                    staffPagesCount = (int)Math.Ceiling(double.Parse(staffCntStr[..(staffCntStr.IndexOf(' '))]) / 100d);
                }
                var staffDiv = _htmlDoc.DocumentNode.SelectNodes("//a[@class='user user--big']");

                for (int i = 0; i < staffDiv.Count; i++)
                {
                    Staff staffInfo = new Staff();
                    staffInfo.Name = staffDiv[i].SelectSingleNode("span[2]/span[1]").InnerText.Trim();
                    staffInfo.Post = staffDiv[i].SelectSingleNode("span[2]/span[2]").InnerText.Trim();
                    var attributeValue = staffDiv[i].SelectSingleNode("span[1]").GetAttributeValue("style", "");

                    string avatarUrl;

                    if (attributeValue != "")
                    {
                        avatarUrl = attributeValue[attributeValue.IndexOf('/')..(attributeValue.Length - 2)];
                        Task.Run(async () => staffInfo.Avatar = await _netUtil.GetImage(avatarUrl));
                    }
                    yield return staffInfo;
                }

                pageNumber++;
            } while (pageNumber <= staffPagesCount);
        }
    }
}
