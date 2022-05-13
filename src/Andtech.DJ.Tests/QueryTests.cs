using Andtech.DJ;
using NUnit.Framework;

namespace DJ.Tests
{

	public class QueryTests
	{

		[Test]
		public void ParseTitle()
		{
			var query = Query.Parse("leper messiah");

			Assert.AreEqual("leper messiah", query.Title);
		}

		[Test]
		public void ParseArtist()
		{
			var query = Query.Parse("leper messiah by metallica");

			Assert.AreEqual("metallica", query.Artist);
		}

		[Test]
		public void ParseAlbum()
		{
			var query = Query.Parse("leper messiah from master of puppets");

			Assert.AreEqual("master of puppets", query.Album);
		}

		[Test]
		public void ParseAll()
		{
			var query = Query.Parse("leper messiah by metallica from master of puppets");

			Assert.AreEqual("leper messiah", query.Title);
			Assert.AreEqual("metallica", query.Artist);
			Assert.AreEqual("master of puppets", query.Album);
		}
	}
}