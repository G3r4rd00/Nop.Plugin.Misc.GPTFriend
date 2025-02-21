using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Nop.Plugin.Misc.GPTFriend.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ToOrderedList<T>(this IQueryable<T> source, string columnName, string order)
        {
            Type t = typeof(T);
            PropertyInfo p = t.GetProperty(columnName);
            if (order == "asc")
                source = source.OrderBy(a => p.GetValue(a));
            else
                source = source.OrderByDescending(a => p.GetValue(a));

            return source;
        }

        public static IQueryable<T> FilterContainsBy<T>(this IQueryable<T> source, string columnName, string value)
        {
            Type t = typeof(T);
            PropertyInfo p = t.GetProperty(columnName);
            source = source.Where(a => ((string)p.GetValue(a)).Contains(value));
            
            return source;
        }

    }
}
