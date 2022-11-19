using Andtech.Common;
using System;
using System.Diagnostics;
using System.IO;

namespace Andtech.DJ
{

	/// <summary>
	/// Finds music files.
	/// </summary>
	public class LibrarySearcher
	{
		public string MusicDirectory { get; set; } = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
		public bool UseMetadata { get; set; } = true;
		public TimeSpan SearchDuration { get; private set; }

		private readonly SongRequest request;

		public LibrarySearcher(string query) : this(SongRequest.Parse(query)) { }

		public LibrarySearcher(SongRequest request)
		{
			this.request = request;
		}

		public bool Search(out AudioFile audioFile)
		{
			var sw = Stopwatch.StartNew();
			var path = Search(request);
			sw.Stop();

			SearchDuration = sw.Elapsed;
			if (string.IsNullOrEmpty(path))
			{
				audioFile = null;
				return false;
			}

			audioFile = AudioFile.Read(path, false);

			var parent = Path.GetDirectoryName(audioFile.Path);
			var grandparent = Path.GetDirectoryName(parent);

			int depth = 0;
			if (!Path.Equals(parent, MusicDirectory))
			{
				depth++;
				if (!Path.Equals(grandparent, MusicDirectory))
				{
					depth++;
				}
			}

			if (depth == 2)
			{
				audioFile.Album = Path.GetFileNameWithoutExtension(parent);
				audioFile.Artist = Path.GetFileNameWithoutExtension(grandparent);
			}
			if (depth == 1)
			{
				audioFile.Artist = Path.GetFileNameWithoutExtension(parent);
			}

			return true;
		}

		private string Search(SongRequest request)
		{
			string songPath;

			var root = new FileSystemScanner(MusicDirectory);
			if (request.HasArtist && request.HasAlbum)
			{
				Log.WriteLine("Searching by artist + album + title...", Verbosity.verbose);
				if (root.FindDirectory(request.Artist).FindDirectory(request.Album).File(request.Title, out songPath))
				{
					return songPath;
				}
				else if (TestFallback())
				{
					return songPath;
				}
			}

			if (request.HasArtist)
			{
				Log.WriteLine("Searching by artist + title...", Verbosity.verbose);
				if (root.FindDirectory(request.Artist).File(request.Title, out songPath))
				{
					return songPath;
				}
				else if (TestFallback())
				{
					return songPath;
				}
			}

			if (request.HasAlbum)
			{
				Log.WriteLine("Searching by album + title...", Verbosity.verbose);
				if (root.FindDirectory(request.Album).File(request.Title, out songPath))
				{
					return songPath;
				}
				else if (TestFallback())
				{
					return songPath;
				}
			}

			Log.WriteLine("Searching by title...", Verbosity.verbose);
			if (root.File(request.Title, out songPath))
			{
				return songPath;
			}
			else if (TestFallback())
			{
				return songPath;
			}

			return null;

			bool TestFallback()
			{
				if (!string.IsNullOrEmpty(songPath))
				{
					Log.WriteLine("Not found directly. Using best match...", ConsoleColor.Yellow, Verbosity.verbose);
					return true;
				}

				return false;
			}
		}
	}
}
