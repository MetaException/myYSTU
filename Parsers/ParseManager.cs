using myYSTU.Model;
using myYSTU.Utils;

namespace myYSTU.Parsers
{
    class ParseManager
    {
        public bool isError = false;
        private static INetUtils _netUtil = DependencyService.Get<INetUtils>();

        public IAsyncEnumerable<IParsable> ParseInfo(IParser parser, string url)
        {
            var _htmlDoc = _netUtil.GetHtmlDoc(url).Result;

            if (_htmlDoc is null)
            {
                isError = true;
                return null;
            }

            //Q:либо парсить тут и возвращать список данных
            /*
            List<IParsable> result = new List<IParsable>();
            await foreach (IParsable info in parsedInfo)
            {
                if (info is null)
                {
                    //Стоит ли выводить информацию которая всё-таки смогла загрузиться
                    isError = true;
                    break;
                }
                result.Add(info);
            }

            return result;
            */

            var parsedInfo = parser.ParseInfo(_htmlDoc);
            return parsedInfo;
        }
    }
}
