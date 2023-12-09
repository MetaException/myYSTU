using HtmlAgilityPack;
using myYSTU.Model;

namespace myYSTU.Parsers
{
    public class GradesParser : IParser
    {
        public async IAsyncEnumerable<IParsable> ParseInfo(HtmlDocument _htmlDoc)
        {
            var gradesTable = _htmlDoc.DocumentNode.SelectSingleNode("//table[2]").SelectNodes("tr");

            //TODO: обработать когда нет оценок
            foreach (var grade in gradesTable)
            {
                Grades subjectInfo = new Grades();

                subjectInfo.Name = grade.SelectSingleNode("td[4]").InnerText.Trim('*', ' ');
                subjectInfo.Type = grade.SelectSingleNode("td[5]").InnerText.Trim();

                if (subjectInfo.Type == "зачет")
                {
                    string t = grade.SelectSingleNode("td[8]").InnerText.Trim();
                    if (!string.IsNullOrEmpty(t))
                        subjectInfo.Grade = "×";
                    if (t == "зачет")
                        subjectInfo.Grade = "✓";
                }
                else
                    subjectInfo.Grade = grade.SelectSingleNode("td[7]").InnerText.Trim();

                subjectInfo.SemesterNumber = grade.SelectSingleNode("td[3]").InnerText[0] - '0';

                yield return subjectInfo;
            }
        }
    }
}
