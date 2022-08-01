using NUnit.Framework;

namespace Andtech.DJ.Tests
{

	public class SongRequestTests
	{

		[Test]
		public void ParseTitle()
		{
			var query = SongRequest.Parse("leper messiah");

			Assert.AreEqual("leper messiah", query.Title);
		}

		[Test]
		public void ParseArtist()
		{
			var query = SongRequest.Parse("leper messiah by metallica");

			Assert.AreEqual("metallica", query.Artist);
		}

		[Test]
		public void ParseAlbum()
		{
			var query = SongRequest.Parse("leper messiah from master of puppets");

			Assert.AreEqual("master of puppets", query.Album);
		}

		[Test]
		public void ParseAll()
		{
			var query = SongRequest.Parse("leper messiah by metallica from master of puppets");

			Assert.AreEqual("leper messiah", query.Title);
			Assert.AreEqual("metallica", query.Artist);
			Assert.AreEqual("master of puppets", query.Album);
		}
	}
}