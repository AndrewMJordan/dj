using Andtech.DJ;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Andtech.Common.Text.SentenceExpressions
{

	/// <summary>
	/// A set of words (strings). Words in a <see cref="Sentence"/> contain metadata such as parenthesization.
	/// </summary>
	public class Sentence : IEnumerable<Word>
	{
		public List<Word> Words => words;

		private readonly List<Word> words = new List<Word>();

		public static Sentence Parse(string text)
		{
			var sentence = new Sentence();
			var isInParentheses = false;
			foreach (var token in Macros.Tokenize(text))
			{
				if (token.StartsWith("("))
				{
					isInParentheses = true;
				}

				var value = Regex.Replace(token, @"(^\()|(\)$)", string.Empty);
				var word = new Word(value, isInParentheses);
				sentence.words.Add(word);

				if (token.EndsWith(")"))
				{
					isInParentheses = false;
				}
			}

			return sentence;
		}

		public IEnumerator<Word> GetEnumerator()
		{
			return ((IEnumerable<Word>)words).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)words).GetEnumerator();
		}
	}
}
