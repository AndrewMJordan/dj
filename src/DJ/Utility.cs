using CaseExtensions;
using Humanizer;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech
{

    class Utility
    {

        public static string Standardize(string x)
        {
            return x.Humanize(LetterCasing.LowerCase);
        }

        public static IEnumerable<string> SplitCommand(string command)
        {
            var regex = new Regex(@"(?<match>[\w-]+)|\""(?<match>[\w\s-]*)""|'(?<match>[\w\s-]*)'");
            return
                from match in regex.Matches(command)
                select match.Groups["match"].Value;
        }
    }
}
