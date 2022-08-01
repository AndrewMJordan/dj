using NUnit.Framework;

namespace Andtech.DJ.Tests
{

	public class StandardizationTests
	{

		[Test]
		public void ParseEmpty()
		{
			var result = Macros.Standardize(string.Empty);

			Assert.AreEqual(string.Empty, result);
		}

		[Test]
		public void ParseSingle()
		{
			var result = Macros.Standardize("alpha");

			Assert.AreEqual("alpha", result);
		}

		[Test]
		public void ParseMultiple()
		{
			var result = Macros.Standardize("alpha bravo gamma");

			Assert.AreEqual("alpha bravo gamma", result);
		}

		[Test]
		public void ParseMixedCase()
		{
			var result = Macros.Standardize("aLpHAbeTiCAL");

			Assert.AreEqual("alphabetical", result);
		}

		[Test]
		public void ParseWhitespace()
		{
			var result = Macros.Standardize("alpha		   bravo");

			Assert.AreEqual("alpha bravo", result);
		}

		[Test]
		public void ParseTriviaCharacters()
		{
			var result = Macros.Standardize("...and justice for all - 1987 rough mix");

			Assert.AreEqual("and justice for all 1987 rough mix", result);
		}

		[Test]
		public void ParseUnderscore()
		{
			var result = Macros.Standardize("alpha_bravo");

			Assert.AreEqual("alpha bravo", result);
		}

		[Test]
		public void ParseDash()
		{
			var result = Macros.Standardize("alpha-bravo");

			Assert.AreEqual("alpha bravo", result);
		}
	}
}