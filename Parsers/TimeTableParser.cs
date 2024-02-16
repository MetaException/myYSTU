using HtmlAgilityPack;
using myYSTU.Model;

namespace myYSTU.Parsers
{
    public class TimeTableParser : AbstractParser<TimeTableSubject>
    {
        private string IDraspz;
        private string idgr;

        public TimeTableParser(string linkToParse)
        {
            _linkToParse = linkToParse;
        }

        private void GetTimeTableParameters()
        {
            string timeTableLink = Links.TimeTableLinkParams;

            //Получаем параметры для запроса расписания
            var timeTableLinq = timeTableLink[(timeTableLink.IndexOf('=') + 1)..];

            IDraspz = timeTableLinq[..timeTableLinq.IndexOf('&')];
            timeTableLinq = timeTableLinq[(timeTableLinq.IndexOf('=') + 1)..];
            idgr = timeTableLinq[..timeTableLinq.IndexOf('&')];
        }

        protected override HttpContent GetPostContent(string date)
        {
            if (IDraspz == null || idgr == null)
            {
                GetTimeTableParameters();
            }

            //Получение на день
            var content = new MultipartFormDataContent
            {
                //{ new StringContent(week), "nned" },
                { new StringContent("-->"), "rgrday" },
                { new StringContent(date), "dat1day" },
                { new StringContent(IDraspz!), "IDraspz" },
                { new StringContent(idgr!), "idgr" },
                //{ new StringContent("-35"), "namegr" }
            };

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

            return content;
        }

        protected override List<TimeTableSubject> ParseHtml(HtmlDocument htmlDoc)
        {
            var subjectsData = htmlDoc.DocumentNode.SelectSingleNode("//table").SelectNodes("tr").SkipLast(1);

            var subjects = new List<TimeTableSubject>();

            foreach (var t in subjectsData)
            {
                var subjectInfo = new TimeTableSubject();

                var interval = t.ChildNodes[0].InnerText[3..];
                subjectInfo.StartTime = interval[..5];
                subjectInfo.EndTime = interval[6..];
                subjectInfo.Name = t.ChildNodes[1].InnerText;
                subjectInfo.Type = t.ChildNodes[2].InnerText;
                subjectInfo.Audithory = t.ChildNodes[3].InnerText.Trim('*', ' ');
                subjectInfo.Lecturer = t.ChildNodes[4].InnerText.Trim();

                subjects.Add(subjectInfo);
            }

            return subjects;
        }
    }
}
