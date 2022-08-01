using NUnit.Framework;

namespace Andtech.DJ.Tests
{

	public class SentenceTests
	{

		[Test]
		public void ParseSimple()
		{
			var sentence = Sentence.Parse("Uptown Funk");

			CollectionAssert.AreEqual(new string[] { "Uptown", "Funk" }, sentence.Words);
		}

		[Test]
		public void ParseWithParentheses()
		{
			var sentence = Sentence.Parse("Uptown Funk (Radio Edit)");

			CollectionAssert.AreEqual(new string[] { "Uptown", "Funk" }, sentence.NonParenthesizedWords);
			CollectionAssert.AreEqual(new string[] { "Radio", "Edit" }, sentence.ParenthesizedWords);
		}

		[Test]
		public void ParseWithSeparateParentheses()
		{
			var sentence = Sentence.Parse("(2020 Remix) Uptown Funk (ft. Bruno Mars)");

			CollectionAssert.AreEqual(new string[] { "Uptown", "Funk" }, sentence.NonParenthesizedWords);
			CollectionAssert.AreEqual(new string[] { "2020", "Remix", "ft.", "Bruno", "Mars" }, sentence.ParenthesizedWords);
		}
	}
}