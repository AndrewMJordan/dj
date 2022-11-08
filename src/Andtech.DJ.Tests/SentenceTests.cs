using NUnit.Framework;
using System.Linq;

namespace Andtech.Common.Text.SentenceExpressions.Tests
{

	public class SentenceTests
	{

		[Test]
		public void ParseSimple()
		{
			var sentence = Sentence.Parse("Uptown Funk");

			CollectionAssert.AreEqual(new string[] { "Uptown", "Funk" }, sentence
				.Words
				.Select(x => x.RawText));
		}

		[Test]
		public void ParseWithParentheses()
		{
			var sentence = Sentence.Parse("Uptown Funk (Radio Edit)");

			CollectionAssert.AreEqual(new string[] { "Uptown", "Funk" }, sentence
				.Words
				.Where(x => !x.IsParenthesized)
				.Select(x => x.RawText));
			CollectionAssert.AreEqual(new string[] { "Radio", "Edit" }, sentence
				.Words
				.Where(x => x.IsParenthesized)
				.Select(x => x.RawText));
		}

		[Test]
		public void ParseWithSeparateParentheses()
		{
			var sentence = Sentence.Parse("(2020 Remix) Uptown Funk (ft. Bruno Mars)");

			CollectionAssert.AreEqual(new string[] { "Uptown", "Funk" }, sentence
				.Words
				.Where(x => !x.IsParenthesized)
				.Select(x => x.RawText));
			CollectionAssert.AreEqual(new string[] { "2020", "Remix", "ft.", "Bruno", "Mars" }, sentence
				.Words
				.Where(x => x.IsParenthesized)
				.Select(x => x.RawText));
		}

		[Test]
		public void ParseWithApostrophe()
		{
			var sentence = Sentence.Parse("Don't Stop Believing");

			CollectionAssert.AreEqual(new string[] { "Don't", "Stop", "Believing" }, sentence
				.Words
				.Where(x => !x.IsParenthesized)
				.Select(x => x.RawText));
		}
	}
}