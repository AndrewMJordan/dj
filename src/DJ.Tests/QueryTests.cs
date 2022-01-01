using Andtech.DJ;
using NUnit.Framework;
using System;
using System.IO;

namespace DJ.Tests
{

	public class QueryTests
	{

		[SetUp]
		public void SetUp()
		{
			Environment.SetEnvironmentVariable("XDG_MUSIC_DIR", Path.GetFullPath("TestFiles"));
		}

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
	}
}