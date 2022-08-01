using System.Collections.Generic;
using System.Linq;

namespace Andtech.DJ
{

	internal class SentenceComparer
	{
		private readonly Sentence sentence;

		public SentenceComparer(Sentence sentence)
		{
			this.sentence = sentence;
		}

		public int CountMatches(IEnumerable<string> tokens) => tokens
			.Select(Macros.Standardize)
			.Count(sentence.Words.Select(Macros.Standardize).Contains);

		public int CountMatchesNonParenthesized(IEnumerable<string> tokens) => tokens
			.Select(Macros.Standardize)
			.Count(sentence.NonParenthesizedWords.Select(Macros.Standardize).Contains);

		public int CountMatchesParenthesized(IEnumerable<string> tokens) => tokens
			.Select(Macros.Standardize)
			.Count(sentence.ParenthesizedWords.Select(Macros.Standardize).Contains);
	}
}
