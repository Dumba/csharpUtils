using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules
{
    /// <summary>
    /// Resolving conditions
    /// format: ((A == B)&(C isEmpty))|(D isNotEmpty)
    /// </summary>
    public interface ICondition
    {
        string GetCondition();
    }

    public static class IConditionExtend
    {
        public static void CheckCondition(this ICondition conditionItem, Dictionary<string, object> variables)
        {
            string condition = conditionItem.GetCondition();
            if (!resolveSet(condition, variables))
                throw new UnauthorizedAccessException();
        }

        #region Resolve
        private static bool resolveSet(string condition, Dictionary<string, object> variables)
        {
            //// is not set anymore
            if (!condition.StartsWith("("))
                return resolve(condition, variables);

            //// set
            bool result = true;
            char operation = '&';
            while (operation == '&' || operation == '|')
            {
                // resolve condition
                int rightBracketIndex = findRightBracket(condition);
                bool currentResult = resolveSet(condition.Substring(1, rightBracketIndex - 1), variables);
                result = operation == '&'
                    ? result && currentResult
                    : result || currentResult;

                // end
                if (rightBracketIndex + 1 >= condition.Length)
                    break;

                // prepare next step
                operation = condition[rightBracketIndex + 1];
                condition = condition.Substring(rightBracketIndex + 2);
            }

            return result;
        }
        private static bool resolve(string condition, Dictionary<string, object> variables)
        {
            string[] singleOperator = new string[] { "isempty", "isnotempty" };
            string[] dualOperator = new string[] { "==", "!=", ">", ">=", "<", "<=", "contains", };

            if (string.IsNullOrWhiteSpace(condition))
                return true;

            string[] splitted = condition.Split(' ');
            string operation = splitted[1].ToLower();

            if (!((singleOperator.Contains(operation) && splitted.Length == 2) || (dualOperator.Contains(operation) && splitted.Length == 3)))
                throw new FormatException();

            int compare;
            switch (splitted[1].ToLower())
            {
                case "==":
                    return Equals(parseValue(splitted[0], variables), parseValue(splitted[2], variables));

                case "!=":
                    return !Equals(parseValue(splitted[0], variables), parseValue(splitted[2], variables));

                case ">":
                    compare = (parseValue(splitted[0], variables) as IComparable).CompareTo(parseValue(splitted[2], variables));
                    return compare > 0;

                case ">=":
                    compare = (parseValue(splitted[0], variables) as IComparable).CompareTo(parseValue(splitted[2], variables));
                    return compare > 0 || compare == 0;

                case "<":
                    compare = (parseValue(splitted[0], variables) as IComparable).CompareTo(parseValue(splitted[2], variables));
                    return compare < 0;

                case "<=":
                    compare = (parseValue(splitted[0], variables) as IComparable).CompareTo(parseValue(splitted[2], variables));
                    return compare < 0 || compare == 0;

                case "isempty":
                    return parseValue(splitted[0], variables) == null;

                case "isnotempty":
                    return parseValue(splitted[0], variables) != null;

                case "contains":
                    if (splitted.Length == 2)
                        throw new FormatException();
                    return (parseValue(splitted[0], variables) as string).Contains((string)parseValue(splitted[2], variables));

                default:
                    throw new FormatException();
            }
        }

        private static object parseValue(string input, Dictionary<string, object> vars)
        {
            // value
            if (input.Length > 1 && input[1] == '$')
                return Convertor.convert(input[0], input.Substring(2));

            // variable
            return GetChainedProperty(input, vars);
        }

        private static int findRightBracket(string item, int startIndex = 0)
        {
            int count = 0;
            for (int i = startIndex; i < item.Length; i++)
            {
                switch (item[i])
                {
                    case '(':
                        count++;
                        break;
                    case ')':
                        count--;
                        break;
                }

                if (count == 0)
                    return i;
            }

            return -1;
        }
        #endregion

        #region chained
        private static object GetChainedProperty(string chainedKey, Dictionary<string, object> vars)
        {
            int index = chainedKey.IndexOfAny(new char[] { '.', '[' });
            if (index == -1)
            {
                if (vars.ContainsKey(chainedKey))
                    return vars[chainedKey];
                else
                    throw new MissingFieldException($"Missing key '{chainedKey}' on list [{vars.ToString()}].");
            }

            string key = chainedKey.Substring(0, index);
            if (key == "__Model" || key == "__Result")
                return vars[chainedKey];
            return GetChained(vars[key], chainedKey.Substring(index));
        }

        private static object GetChained(object o, string calling)
        {
            string[] propertyCallings = calling.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

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
            var prop = o.GetType().GetMethod("get_Item", new Type[] { typeof(string) });
            return prop.Invoke(o, new object[] { calling });
        }

        private static object resolveSingleProperty(object o, string calling)
        {
            //if (o is DBItem && ((DBItem)o).getColumnNames().Contains(calling))
            //    return ((DBItem)o)[calling];

            PropertyInfo property = o.GetType().GetProperty(calling);

            if (property == null)
                throw new MissingFieldException($"Missing field '{calling}' on item [{o.ToString()}].");

            return property.GetValue(o);
        }
        #endregion
    }

    public class Convertor
    {
        public static object convert(char shortcut, object input)
        {
            switch (shortcut)
            {
                case 'i':
                    return Convert.ToInt32(input);
                case 'b':
                    return Convert.ToBoolean(input);
                case 's':
                    return Convert.ToString(input);
                case 'f':
                    return Convert.ToDouble(input);
                case 'd':
                    return Convert.ToDateTime(input);
                case 'l':
                    return input;
                // none
                default:
                    throw new KeyNotFoundException($"Cannot indentify data type '{shortcut}'");
            }
        }
    }
}
