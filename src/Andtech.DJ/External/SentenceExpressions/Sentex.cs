using Andtech.DJ;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.Common.Text.SentenceExpressions
{

	/// <summary>
	/// Sentence expression.
	/// </summary>
	public class Sentex
	{
		class _Term
		{
			public string Text { get; set; }
			public Regex Regex { get; set; }
		}

		private readonly Sentence sentence;

		public Sentex(string pattern)
		{
			sentence = Sentence.Parse(pattern);
		}

		public TermCollection Match(string pattern)
		{
			var terms = Terms(pattern, out var success);
			return new TermCollection()
			{
				Terms = terms,
				Success = success,
			};
		}

		public bool IsMatch(string pattern)
		{
			Terms(pattern, out var success);
			return success;
		}

		Term[] Terms(string pattern, out bool success)
		{
			var n = sentence.Words.Count;
			var terms = new Term[n];
			for (int i = 0; i < n; i++)
			{
				terms[i] = new Term(sentence.Words[i]);
			}

			var otherWords = Macros
				.Tokenize(pattern)
				.ToList();
			var m = otherWords.Count;

			int index = 0;
			bool skippedAny = false;
			int matchCount = 0;

			for (int j = 0; j < m; j++)
			{
				for (int i = index; i < n; i++)
				{
					var isSubstring = Regex.IsMatch(sentence.Words[i].Value, $@"^{otherWords[j]}[^\s]*");
					if (isSubstring)
					{
						terms[i].Success = true;
						index = i + 1;
						matchCount++;
						goto End;
					}
				}
				skippedAny |= true;

			End:
				{ }
			}

			success = matchCount == m && !skippedAny;
			return terms;
		}
	}
}
