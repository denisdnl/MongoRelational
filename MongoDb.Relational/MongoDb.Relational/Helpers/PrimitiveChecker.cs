using System;
using System.Linq;
using System.Reflection;

namespace MongoDb.Relational.Helpers
{
    internal class PrimitiveChecker
    {
        public static bool IsPrimitive(PropertyInfo p)
        {
            var type = p.PropertyType;
            return type.IsPrimitive ||
            new Type[] {
                  typeof(string),
                  typeof(decimal),
                  typeof(DateTime),
                  typeof(DateTimeOffset),
                  typeof(TimeSpan),
                  typeof(Guid)
            }.Contains(type) ||
            type.IsEnum;
        }
    }
}
