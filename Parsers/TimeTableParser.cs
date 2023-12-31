﻿using myYSTU.Model;
using myYSTU.Utils;

namespace myYSTU.Parsers
{
    public class TimeTableParser
    {
        private string IDraspz;
        private string idgr;
        private string timeTableLink = Links.TimeTableLinkParams;

        private readonly NetUtils _netUtils = DependencyService.Get<NetUtils>();

        private async Task GetTimeTableParameters()
        {
            //Получаем параметры для запроса расписания
            var timeTableLinq = timeTableLink[(timeTableLink.IndexOf('=') + 1)..];

            IDraspz = timeTableLinq[..timeTableLinq.IndexOf('&')];
            timeTableLinq = timeTableLinq[(timeTableLinq.IndexOf('=') + 1)..];
            idgr = timeTableLinq[..timeTableLinq.IndexOf('&')];
        }

        /*
        //Получение на неделю
        var content = new MultipartFormDataContent
        {
            { new StringContent(date), "nned" },
            { new StringContent("-->"), "rgrweek" },
            //{ new StringContent("19.10.2023"), "dat1day" },
            { new StringContent(IDraspz), "IDraspz" },
            { new StringContent(idgr), "idgr" },
            //{ new StringContent("-35"), "namegr" }
        };*/

        public async IAsyncEnumerable<TimeTableSubject> ParseInfoByDay(string date)
        {
            //?
            if (IDraspz == null || idgr == null)
            {
                await GetTimeTableParameters();
            }

            //Получение на день
            var content = new MultipartFormDataContent
            {
                //{ new StringContent(week), "nned" },
                { new StringContent("-->"), "rgrday" },
                { new StringContent(date), "dat1day" },
                { new StringContent(IDraspz), "IDraspz" },
                { new StringContent(idgr), "idgr" },
                //{ new StringContent("-35"), "namegr" }
            };

            var timeTableHtmlByDay = await _netUtils.GetHtmlDoc(Links.TimeTableLink, content);

            var subjectsData = timeTableHtmlByDay.DocumentNode.SelectSingleNode("//table").SelectNodes("tr").SkipLast(1);

            foreach (var t in subjectsData)
            {
                var subjectInfo = new TimeTableSubject();

                var interval = t.ChildNodes[0].InnerText[3..];
                subjectInfo.StartTime = interval[..5];
                subjectInfo.EndTime = interval[6..];
                subjectInfo.Name = t.ChildNodes[1].InnerText;
                subjectInfo.Type = t.ChildNodes[2].InnerText;
                subjectInfo.Audithory = t.ChildNodes[3].InnerText.Trim('*',' ');
                subjectInfo.Lecturer = t.ChildNodes[4].InnerText.Trim();

                yield return subjectInfo;
            }
        }
    }
}
