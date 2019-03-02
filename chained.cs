using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class chained
    {
        private static object GetChained(object o, string calling)
        {
            string[] propertyCallings = calling.Split('.');

            foreach (string propertyCalling in propertyCallings)
            {
                o = resolveProperty(o, propertyCalling);
            }

            return o;
        }

        private static object resolveProperty(object o, string calling)
        {
            int iStart = calling.IndexOf('[');

            // no array
            if (iStart < 0)
                return resolveSingleProperty(o, calling);

            // array with property
            if (iStart > 0)
            {
                o = resolveSingleProperty(o, calling.Substring(0, iStart));
                calling = calling.Substring(iStart);
                iStart = 0;
            }

            int iEnd = calling.IndexOf(']', iStart);
            // single array
            if (iEnd == calling.Length - 1)
                return resolveSingleArray(o, calling.Substring(1, calling.Length - 2));

            // multiple arrays
            while (iStart >= 0)
            {
                o = resolveSingleArray(o, calling.Substring(iStart + 1, iEnd - iStart - 1));
                iStart = calling.IndexOf('[', iEnd);
                iEnd = iStart >= 0 ? calling.IndexOf(']', iStart) : -1;
            }

            return o;
        }

        private static object resolveSingleArray(object o, string calling)
        {
            var prop = o.GetType().GetMethod("get_Item");
            return prop.Invoke(o, new object[] { calling });
        }

        private static object resolveSingleProperty(object o, string calling)
        {
            PropertyInfo property = o.GetType().GetProperty(calling);

            if (property == null)
                throw new MissingFieldException($"Missing field '{calling}' on item [{o.ToString()}].");

            return property.GetValue(o);
        }
    }
}
