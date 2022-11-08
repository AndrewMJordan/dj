using Andtech.Common;
using System;
using System.Diagnostics;
using System.IO;

namespace Andtech.DJ
{

	/// <summary>
	/// Finds music files.
	/// </summary>
	public class FileSystemSearcher
	{
		public string MusicDirectory { get; set; } = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
		public bool UseMetadata { get; set; } = true;
		public TimeSpan SearchDuration { get; private set; }

		private readonly SongRequest request;
		private readonly MusicScanner scanner;

		public FileSystemSearcher(string query) : this(SongRequest.Parse(query)) { }

		public FileSystemSearcher(SongRequest request)
		{
			this.request = request;
			scanner = new MusicScanner(request);
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
