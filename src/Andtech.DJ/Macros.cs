using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.DJ
{

	public static class Macros
	{

		/// <summary>
		/// Creates an <see cref="Sentence"/> from the string <paramref name="text"/>.
		/// </summary>
		/// <param name="text">The text representing the sentence.</param>
		/// <returns>The sentence.</returns>
		public static Sentence ToSentence(string text)
		{
			text = Standardize(text);
			return Sentence.Parse(text);
		}

		/// <summary>
		/// Convert the string to a standard queryable format.
		/// </summary>
		/// <param name="text">The string to standardize.</param>
		/// <returns>The standardized query string.</returns>
		public static string Standardize(string text)
		{
			text = text.ToLower();
			text = Regex.Replace(text, @"[^\w\s]", string.Empty);
			text = Regex.Replace(text, @"\s+", " ");
			text = text.Trim();

			return text;
		}

		/// <summary>
		/// Split the query based on a standard format.
		/// </summary>
		/// <param name="text">The query string.</param>
		/// <returns>The tokens of the query.</returns>
		public static IEnumerable<string> Tokenize(string text)
		{
			text = text.Trim();

			if (string.IsNullOrEmpty(text))
			{
				return Enumerable.Empty<string>();
			}

			return Regex.Split(text, @"\s+");
		}
	}
}
