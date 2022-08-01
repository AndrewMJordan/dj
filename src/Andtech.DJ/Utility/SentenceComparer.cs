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

		public int CountMatches(IEnumerable<string> tokens) => tokens.Count(sentence.Words.Contains);

		public int CountMatchesNonParenthesized(IEnumerable<string> tokens) => tokens.Count(sentence.NonParenthesizedWords.Contains);

		public int CountMatchesParenthesized(IEnumerable<string> tokens) => tokens.Count(sentence.ParenthesizedWords.Contains);
	}
}
