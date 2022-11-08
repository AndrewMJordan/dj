using System.Text.RegularExpressions;

namespace Andtech.Common.Text.SentenceExpressions
{

	public class Word
	{
		public string Value { get; set; }
		public Regex PrefixRegex { get; set; }
		public bool IsParenthesized { get; set; }
		public string RawText { get; set; }

		public Word(string text, bool isParenthesized = false)
		{
			Value = Standardize(text);
			RawText = text;

			PrefixRegex = new Regex($@"^{Value}[^\s]*");
			IsParenthesized = isParenthesized;
		}

		public override string ToString() => IsParenthesized ? $"({RawText})" : RawText;

		/// <summary>
		/// Convert the string to a standard queryable format.
		/// </summary>
		/// <param name="text">The string to standardize.</param>
		/// <returns>The standardized query string.</returns>
		public static string Standardize(string text)
		{
			text = text.ToLower();
			text = Regex.Replace(text, @"(_+|-+)", " ");
			text = Regex.Replace(text, @"[^\w\s]", string.Empty);
			text = Regex.Replace(text, @"(\s+)", " ");
			text = text.Trim();

			return text;
		}
	}
}
