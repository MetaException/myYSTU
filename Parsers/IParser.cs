using HtmlAgilityPack;
using myYSTU.Model;

namespace myYSTU.Parsers
{
    interface IParser
    {
        public IAsyncEnumerable<IParsable> ParseInfo(HtmlDocument _htmlDoc);
    }
}
