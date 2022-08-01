using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.DJ
{

	internal class SentenceComparer
	{
		private List<Regex> all;
		private List<Regex> nonParenthesized;
		private List<Regex> parenthesized;

		public SentenceComparer(Sentence query)
		{
			all = query
				.Words
				.Select(Macros.Standardize)
				.Select(GetRegexFromPrefixes)
				.ToList();
			nonParenthesized = query
				.NonParenthesizedWords
				.Select(Macros.Standardize)
				.Select(GetRegexFromPrefixes)
				.ToList();
			parenthesized = query
				.ParenthesizedWords
				.Select(Macros.Standardize)
				.Select(GetRegexFromPrefixes)
				.ToList();
		}

		public int CountMatches(IEnumerable<string> tokens) => CountIt(tokens, all);

		public int CountMatchesNonParenthesized(IEnumerable<string> tokens) => CountIt(tokens, nonParenthesized);

		public int CountMatchesParenthesized(IEnumerable<string> tokens) => CountIt(tokens, parenthesized);

		private int CountIt(IEnumerable<string> tokens, List<Regex> regexes)
		{
			var n = regexes.Count;

			var list = tokens
				.Select(Macros.Standardize)
				.ToList();
			var m = list.Count;

			int j = 0;
			int count = 0;

			int i = 0;
			while (i < n && j < m)
			{
				for (int k = j; k < m; k++)
				{
					if (regexes[i].IsMatch(list[k]))
					{
						count++;
						j = k + 1;
					}
				}

				i++;
			}

			return count;
		}

		static Regex GetRegexFromPrefixes(string prefix)
		{
			return new Regex($@"^{prefix}[^\s]*$");
		}
	}
}
