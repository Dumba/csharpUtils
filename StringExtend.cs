using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public static partial class ExtendMethods
    {
        public static char ToUpper(this char input)
        {
            int i = input;
            if (i >= 97 && i <= 122)
                i -= 32;

            return (char)i;
        }
        public static char ToLower(this char input)
        {
            int i = input;
            if (i >= 65 && i <= 90)
                i += 32;

            return (char)i;
        }
        public static string RemoveDiacritics(this string input, bool firstUpper = true)
        {
            input = input.Normalize(NormalizationForm.FormD);

            // remove diacriticts
            input = new string(input
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
            
            // remove special symbols
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            var splitted = rgx.Split(input)
                .Where(s => s.Length != 0) // remove empty
                .Select(s => (firstUpper ? s[0].ToUpper() : s[0].ToLower()) + s.Substring(1).ToLower()) // good case
                .ToArray();

            // merge
            input = string.Join("", splitted);

            return input;
        }
    }
}
