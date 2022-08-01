using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.DJ
{

	/// <summary>
	/// A set of words (strings). Words in a <see cref="Sentence"/> contain metadata such as parenthesization.
	/// </summary>
	public class Sentence
	{
		public IEnumerable<string> Words => NonParenthesizedWords.Concat(ParenthesizedWords);
		public IEnumerable<string> NonParenthesizedWords { get; set; }
		public IEnumerable<string> ParenthesizedWords { get; set; }

		private static readonly Regex parenthesizedRegex = new Regex(@"\((?<value>.*?)\)");

		public static Sentence Parse(string text)
		{
			var sentence = new Sentence();

			// Get parenthesized
			var parenthesizedMatches = parenthesizedRegex.Matches(text);
			var parenthesizedString = string.Join(" ", parenthesizedMatches.Select(x => x.Groups["value"].Value));
			sentence.ParenthesizedWords = Macros.Tokenize(parenthesizedString);

			// Remove parenthesized
			text = parenthesizedRegex.Replace(text, string.Empty);
			var nonParenthesizedString = text;
			sentence.NonParenthesizedWords = Macros.Tokenize(nonParenthesizedString);

			return sentence;
		}
	}
}
