using myYSTU.Models;

namespace myYSTU.Parsers;

public static class ParserFactory
{
    public static IParser<T> CreateParser<T>()
    {
        if (typeof(T) == typeof(Grades))
        {
            return (IParser<T>)new GradesParser(Links.GradesLink);
        }
        else if (typeof(T) == typeof(Person))
        {
            return (IParser<T>)new PersonParser(Links.AccountInfoLink);
        }
        else if (typeof(T) == typeof(TimeTableSubject))
        {
            return (IParser<T>)new TimeTableParser(Links.TimeTableLink);
        }
        else if (typeof(T) == typeof(Staff))
        {
            return (IParser<T>)new StaffParser(Links.StaffLink);
        }
        else
        {
            throw new ArgumentException("Unsupported type");
        }
    }
}