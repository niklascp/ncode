using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Linq.Mapping;

namespace nCode.Data.Linq2Sql
{
    public static class Extensions
    {
        public static T Clone<T>(this T source)
        {
            var clone = (T)Activator.CreateInstance(typeof(T));
            var cols = typeof(T).GetProperties()
                .Select(p => new { Prop = p, Attr = (ColumnAttribute)p.GetCustomAttributes(typeof(ColumnAttribute), true).SingleOrDefault() })
                .Where(p => p.Attr != null && !p.Attr.IsDbGenerated);
            foreach (var col in cols)
                col.Prop.SetValue(clone, col.Prop.GetValue(source, null), null);
            return clone;
        }
    }
}
