using myYSTU.Model;
using myYSTU.Utils;

namespace myYSTU.Parsers
{
    public static class TimeTableParser
    {
        private static INetUtils _netUtil = DependencyService.Get<INetUtils>();

        private static string IDraspz;
        private static string idgr;
        private static string timeTableLink;

        private static async Task ParseTimeTableParameters()
        {
            //Получаем ссылку на расписание
            var _htmlDoc = await _netUtil.GetHtmlDoc("/WPROG/lk/lkstud.php");
            timeTableLink = _htmlDoc.DocumentNode.SelectSingleNode("//div[1]/div[1]/div[4]/div[1]/font[1]/table[1]/tr[1]/td[2]/font[1]/i[1]/a[1]").GetAttributeValue("href", "");

            //Получаем параметры для запроса расписания
            var timeTableLinq = timeTableLink[(timeTableLink.IndexOf('=') + 1)..];

            IDraspz = timeTableLinq[..timeTableLinq.IndexOf('&')];
            timeTableLinq = timeTableLinq[(timeTableLinq.IndexOf('=') + 1)..];
            idgr = timeTableLinq[..timeTableLinq.IndexOf('&')];
        }

        public static async Task<DateTime[]> ParseWeekList()
        {
            if (timeTableLink == null)
            {
                await ParseTimeTableParameters();
            }

            //Получаем расписание на семестр
            var _htmlDoc = await _netUtil.GetHtmlDoc(timeTableLink);

            //Получаем список неделей с датами
            var weeks = _htmlDoc.DocumentNode.SelectNodes("//option");

            List<DateTime> w = new List<DateTime>();

            foreach (var week in weeks)
            {
                string value = week.GetAttributeValue("value", "");
                w.Add(DateTime.Parse(value[(value.IndexOf('-') + 1)..]));
            }
            return w.ToArray();
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

        public static async IAsyncEnumerable<TimeTableSubject> ParseInfoByDay(string date)
        {
            if (IDraspz == null || idgr == null)
            {
                await ParseTimeTableParameters();
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

            var timeTableHtmlByDay = await _netUtil.PostWebData("/wprog/rasp/raspz1day.php", multipartFormDataContent: content);

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
