using NUnit.Framework;
using System;
using System.IO;

namespace Andtech.DJ.Tests
{

	internal class SearchTests
	{
		private static readonly string MusicDirectory = Path.GetFullPath("TestFiles");

		[Test]
		public void FindSong()
		{
			var searcher = new LibrarySearcher("master of puppets")
			{
				MusicDirectory = MusicDirectory,
				UseMetadata = true,
			};

			if (searcher.Search(out var audioFile))
			{
				AreSamePath("Metallica/Master of Puppets/Master of Puppets.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		[Test]
		public void FindSongFromPrefixes()
		{
			var searcher = new LibrarySearcher("mast pup")
			{
				MusicDirectory = MusicDirectory,
				UseMetadata = true,
			};

			if (searcher.Search(out var audioFile))
			{
				AreSamePath("Metallica/Master of Puppets/Master of Puppets.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		[Test]
		public void FindSongDuplicateMetallica()
		{
			var searcher = new LibrarySearcher("master by metallica")
			{
				MusicDirectory = MusicDirectory,
				UseMetadata = true,
			};

			if (searcher.Search(out var audioFile))
			{
				AreSamePath("Metallica/Master of Puppets/Master of Puppets.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		[Test]
		public void FindSongDuplicateMeatbodies()
		{
			var searcher = new LibrarySearcher("master by meatbodies")
			{
				MusicDirectory = MusicDirectory,
				UseMetadata = true,
			};

			if (searcher.Search(out var audioFile))
			{
				AreSamePath("Meatbodies/The Master.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		[Test]
		public void FindSongIgnoreTrivia()
		{
			var searcher = new LibrarySearcher("and by metallica")
			{
				MusicDirectory = MusicDirectory,
			};

			if (searcher.Search(out var audioFile))
			{
				AreSamePath("Metallica/...And Justice for All/...And Justice for All.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		[Test]
		public void FindSongIgnoreParentheses()
		{
			var searcher = new LibrarySearcher("old mcdonald")
			{
				MusicDirectory = MusicDirectory,
			};

			if (searcher.Search(out var audioFile))
			{
				AreSamePath("Old McDonald.mp3", audioFile.Path);
			}
			else
			{
				Assert.Fail();
			}
		}

		[Test]
		public void FindSongWithParenthesesQuery()
		{
			var searcher = new LibrarySearcher("old mcdonald extended")
			{
				MusicDirectory = MusicDirectory,
			};

			if (searcher.Search(out var audioFile))
			{
				AreSamePath("Old McDonald (Extended).mp3", audioFile.Path);
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
