
namespace Andtech.Common.Text.SentenceExpressions
{
	public class Term
	{
		public Word Word { get; internal set; }
		public bool Success { get; internal set; }

		public Term(Word word)
		{
			Word = word;
		}
	}
}
