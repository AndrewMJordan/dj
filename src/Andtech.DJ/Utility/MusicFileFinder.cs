using Andtech.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Andtech.DJ
{

	/// <summary>
	/// Finds music files.
	/// </summary>
	public class MusicFileFinder
	{
		public string MusicDirectory { get; set; } = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
		public bool UseMetadata { get; set; } = true;
		public TimeSpan SearchDuration { get; private set; }

		private readonly SongRequest request;
		private readonly MusicScanner scanner;

		public MusicFileFinder(string query) : this(SongRequest.Parse(query)) { }

		public MusicFileFinder(SongRequest request)
		{
			this.request = request;
			scanner = new MusicScanner(request);
		}

		public static bool IsMatch(SongRequest request, AudioFile audioFile)
		{
			if (!string.IsNullOrEmpty(request.Title))
			{
				var testTitleSentence = Macros.ToSentence(audioFile.Title);
				var requestTitleSentence = Macros.ToSentence(request.Title);
				var titleComparer = new SentenceComparer(requestTitleSentence);

				if (titleComparer.CountMatches(testTitleSentence.Words) < requestTitleSentence.Words.Count())
				{
					return false;
				}
			}
			if (!string.IsNullOrEmpty(request.Artist))
			{
				var testArtistSentence = Macros.ToSentence(audioFile.Artist);
				var requestArtistSentence = Macros.ToSentence(request.Artist);
				var artistComparer = new SentenceComparer(requestArtistSentence);

				if (artistComparer.CountMatches(testArtistSentence.Words) < requestArtistSentence.Words.Count())
				{
					return false;
				}
			}
			if (!string.IsNullOrEmpty(request.Album))
			{
				var testAlbumSentence = Macros.ToSentence(audioFile.Album);
				var requestAlbumSentence = Macros.ToSentence(request.Album);
				var albumComparer = new SentenceComparer(requestAlbumSentence);

				if (albumComparer.CountMatches(testAlbumSentence.Words) < requestAlbumSentence.Words.Count())
				{
					return false;
				}
			}

			return true;
		}

		public bool TryFindMatch(out AudioFile audioFile)
		{
			Log.WriteLine($"Title query is: '{request.Title}'", Verbosity.verbose);
			Log.WriteLine($"Artist query is: '{request.Artist}'", Verbosity.verbose);
			Log.WriteLine($"Album query is: '{request.Album}'", Verbosity.verbose);

			var sw = Stopwatch.StartNew();
			var path = Search(request);
			sw.Stop();

			SearchDuration = sw.Elapsed;
			if (string.IsNullOrEmpty(path))
			{
				audioFile = null;
				return false;
			}

			Log.WriteLine($"Found song '{path}' in {sw.ElapsedMilliseconds} ms", ConsoleColor.Cyan, Verbosity.verbose);
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

			if (SearchByArtistAndAlbum(MusicDirectory, out songPath))
			{
				return songPath;
			}
			else if (SearchByArtist(MusicDirectory, out songPath))
			{
				return songPath;
			}
			else if (SearchByAlbum(MusicDirectory, out songPath))
			{
				return songPath;
			}
			else if (SearchBySong(MusicDirectory, out songPath))
			{
				return songPath;
			}

			return string.Empty;
		}

		public bool SearchByArtistAndAlbum(string searchRoot, out string path)
		{
			if (request.HasAlbum && request.HasArtist)
			{
				Log.WriteLine($"Searching with artist+album...", Verbosity.verbose);
				if (scanner.TryFindDirectory(searchRoot, out var artistRoot, MusicMetadataField.Artist))
				{
					Log.WriteLine($"Found artist root: {artistRoot}", Verbosity.verbose);
					if (scanner.TryFindDirectory(artistRoot, out var albumRoot, MusicMetadataField.Artist))
					{
						Log.WriteLine($"Found album root: {albumRoot}", Verbosity.verbose);
						if (scanner.TryFindFile(albumRoot, out var songPath, MusicMetadataField.Song, SearchOption.AllDirectories))
						{
							path = songPath;
							return true;
						}
					}
				}
			}

			path = default;
			return false;
		}

		public bool SearchByArtist(string searchRoot, out string path)
		{
			if (request.HasArtist)
			{
				Log.WriteLine($"Searching with artist only...", Verbosity.verbose);
				if (scanner.TryFindDirectory(searchRoot, out var artistRoot, MusicMetadataField.Artist))
				{
					Log.WriteLine($"Found artist root: {artistRoot}", Verbosity.verbose);
					if (scanner.TryFindFile(artistRoot, out var songPath, MusicMetadataField.Song, SearchOption.AllDirectories))
					{
						path = songPath;
						return true;
					}
				}
			}

			path = default;
			return false;
		}

		public bool SearchByAlbum(string searchRoot, out string path)
		{
			if (request.HasAlbum)
			{
				Log.WriteLine($"Searching with album only...", Verbosity.verbose);
				if (scanner.TryFindDirectory(searchRoot, out var albumRoot, MusicMetadataField.Album))
				{
					Log.WriteLine($"Found album root: {albumRoot}", Verbosity.verbose);
					if (scanner.TryFindFile(albumRoot, out var songPath, MusicMetadataField.Song, SearchOption.AllDirectories))
					{
						path = songPath;
						return true;
					}
				}
			}

			path = default;
			return false;
		}

		public bool SearchBySong(string searchRoot, out string path)
		{
			Log.WriteLine($"Searching by brute force...", Verbosity.verbose);
			if (scanner.TryFindFile(searchRoot, out var songPath, MusicMetadataField.Song, SearchOption.AllDirectories))
			{
				path = songPath;
				return true;
			}

			path = default;
			return false;
		}
	}
}
