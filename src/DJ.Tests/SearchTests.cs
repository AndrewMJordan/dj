using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.DJ.Tests
{

	internal class SearchTests
	{
		private static readonly string MusicDirectory = Path.GetFullPath("TestFiles");

		[Test]
		public void FindSong()
		{
			var helper = new SearchHelper
			{
				MusicDirectory = MusicDirectory,
				UseMetadata = true,
			};

			if (helper.TryFindMatch("leper messiah", out var audioFile))
			{
				AreSamePath("metallica/master-of-puppets/leper-messiah.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		[Test]
		public void FindSongDuplicateMetallica()
		{
			var helper = new SearchHelper
			{
				MusicDirectory = MusicDirectory,
				UseMetadata = true,
			};

			if (helper.TryFindMatch("master by metallica", out var audioFile))
			{
				AreSamePath("metallica/master-of-puppets/master-of-puppets.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		[Test]
		public void FindSongDuplicateMeatbodies()
		{
			var helper = new SearchHelper
			{
				MusicDirectory = MusicDirectory,
				UseMetadata = true,
			};

			if (helper.TryFindMatch("master by meatbodies", out var audioFile))
			{
				AreSamePath("meatbodies/the-master.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		private static void AreSamePath(string expected, string actual)
		{
			expected = Path.Combine(MusicDirectory, expected);
			actual = Path.GetFullPath(actual);

			Assert.AreEqual(NormalizePath(expected), NormalizePath(actual));
		}

		public static string NormalizePath(string path)
		{
			return Path.GetFullPath(new Uri(path).LocalPath)
					   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
					   .ToUpperInvariant();
		}
	}
}
