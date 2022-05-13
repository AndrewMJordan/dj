using System;
using System.Diagnostics;
using System.IO;

namespace Andtech.DJ
{

	public class SearchHelper
	{
		public string MusicDirectory { get; set; } = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
		public bool UseMetadata { get; set; } = true;
		public TimeSpan SearchTime { get; private set; }

		public bool TryFindMatch(string query, out AudioFile audioFile) => TryFindMatch(Query.Parse(query), out audioFile);

		public bool TryFindMatch(Query query, out AudioFile audioFile)
		{
			Log.WriteLine($"Title query is: '{query.Title}'", Verbosity.verbose);
			Log.WriteLine($"Artist query is: '{query.Artist}'", Verbosity.verbose);
			Log.WriteLine($"Album query is: '{query.Album}'", Verbosity.verbose);

			var sw = Stopwatch.StartNew();
			var path = Find(query);
			sw.Stop();

			SearchTime = sw.Elapsed;
			if (path == null)
			{
				audioFile = null;
				return false;
			}

			Log.WriteLine($"Found song '{path}' in {sw.ElapsedMilliseconds} ms", ConsoleColor.Cyan, Verbosity.verbose);
			audioFile = AudioFile.Read(path, false);
			return true;
		}

		private string Find(Query query)
		{
			var comparer = new QueryComparer(query);

			// Find exact
			var exactPath = Path.Combine(MusicDirectory, query.Raw);
			Log.WriteLine($"Searching by exact path...", Verbosity.verbose);
			if (File.Exists(exactPath))
			{
				return exactPath;
			}

			// Find by artist and album
			if (query.HasAlbum && query.HasArtist)
			{
				Log.WriteLine($"Searching with artist+album...", Verbosity.verbose);
				if (AudioFilePath.GetDirectory(MusicDirectory, out var artistRoot, predicate: comparer.IsArtistMatch))
				{
					Log.WriteLine($"Found artist root: {artistRoot}", Verbosity.verbose);
					if (AudioFilePath.GetDirectory(artistRoot, out var albumRoot, predicate: comparer.IsAlbumMatch))
					{
						Log.WriteLine($"Found album root: {albumRoot}", Verbosity.verbose);
						if (AudioFilePath.GetFile(albumRoot, out var songPath, predicate: comparer.IsTitleMatch))
						{
							return songPath;
						}
					}
				}
			}
			// Find by artist
			else if (query.HasArtist)
			{
				Log.WriteLine($"Searching with artist only...", Verbosity.verbose);
				if (AudioFilePath.GetDirectory(MusicDirectory, out var artistRoot, predicate: comparer.IsArtistMatch))
				{
					Log.WriteLine($"Found artist root: {artistRoot}", Verbosity.verbose);
					if (AudioFilePath.GetFile(artistRoot, out var songPath, predicate: comparer.IsTitleMatch, SearchOption.AllDirectories))
					{
						return songPath;
					}
				}
			}
			// Find by album
			else if (query.HasAlbum)
			{
				Log.WriteLine($"Searching with album only...", Verbosity.verbose);
				if (AudioFilePath.GetDirectory(MusicDirectory, out var albumRoot, predicate: comparer.IsAlbumMatch))
				{
					Log.WriteLine($"Found album root: {albumRoot}", Verbosity.verbose);
					if (AudioFilePath.GetFile(albumRoot, out var songPath, predicate: comparer.IsTitleMatch, SearchOption.AllDirectories))
					{
						return songPath;
					}
				}
			}

			// Find by brute force
			Log.WriteLine($"Searching by brute force...", Verbosity.verbose);
			AudioFilePath.GetFile(MusicDirectory, out var path, predicate: comparer.IsTitleMatch, SearchOption.AllDirectories);

			return path;
		}
	}
}
