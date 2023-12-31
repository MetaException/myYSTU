﻿using myYSTU.Model;
using myYSTU.Utils;
using System.Collections.Concurrent;

namespace myYSTU.Parsers
{
    public class StaffParser
    {
        private readonly NetUtils _netUtils = DependencyService.Get<NetUtils>();

        public async IAsyncEnumerable<ConcurrentBag<Staff>> ParseInfo()
        {
            int pageNumber = 1;
            int staffPagesCount = 2;

            do
            {
                var _htmlDoc = await _netUtils.GetHtmlDoc($"{Links.StaffLink}{pageNumber}");

                if (pageNumber == 1)
                {
                    var staffCntStr = _htmlDoc.DocumentNode.SelectSingleNode("/html/body/section[2]/div/div/div[2]/span").InnerText;
                    staffPagesCount = (int)Math.Ceiling(double.Parse(staffCntStr[..(staffCntStr.IndexOf(' '))]) / 100d);
                }
                var staffDiv = _htmlDoc.DocumentNode.SelectNodes("//a[@class='user user--big']");

                var staffList = new ConcurrentBag<Staff>();
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
                        staffInfo.AvatarUrl = avatarUrl;
                    }
                    else
                    {
                        //TODO: заменить на лого ЯГТУ
                        staffInfo.AvatarUrl = "/upload/resize_cache/webp/iblock/638/neqgj65a8nu4z0y81005sc62nzb4o8r3/220_220_1/zaglushka-m.webp";
                    }

                    staffList.Add(staffInfo);
                }
                yield return staffList;

                pageNumber++;
            } while (pageNumber <= staffPagesCount);
        }
    }
}
