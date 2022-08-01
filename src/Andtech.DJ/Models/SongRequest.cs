using System;
using System.Linq;

namespace Andtech.DJ
{

	/// <summary>
	/// A simple description of a song. Contains additional metadata about the song.
	/// </summary>
	public class SongRequest
	{
		public string Title { get; set; }
		public bool HasTitle => !string.IsNullOrEmpty(Title);
		public string Artist { get; set; }
		public bool HasArtist => !string.IsNullOrEmpty(Artist);
		public string Album { get; set; }
		public bool HasAlbum => !string.IsNullOrEmpty(Album);
		public string Raw { get; set; }

		public static SongRequest Parse(string sentence) => Parse(null, null, null, sentence.Split(" ", StringSplitOptions.RemoveEmptyEntries));

		public static SongRequest Parse(string title, string artist, string album, params string[] tokens)
		{
			var input = string.Join(" ", tokens.Select(x => x.Trim()));

			int n = tokens.Length;

			bool hasTitle = !string.IsNullOrEmpty(title);
			bool hasArtist = !string.IsNullOrEmpty(artist);
			bool hasAlbum = !string.IsNullOrEmpty(album);

			bool foundArtistSplit = TryLastIndexOf(tokens, "by", out var indexOfArtistSplit, 0);
			bool foundAlbumSplit = TryLastIndexOf(tokens, "from", out var indexOfAlbumSplit, foundArtistSplit ? indexOfArtistSplit + 1 : 0);
			if (hasArtist)
			{
				indexOfArtistSplit = n;
			}
			if (hasAlbum)
			{
				indexOfAlbumSplit = n;
			}
			indexOfArtistSplit = Math.Min(indexOfArtistSplit, indexOfAlbumSplit);
			indexOfAlbumSplit = Math.Max(indexOfArtistSplit, indexOfAlbumSplit);

			if (!hasTitle)
			{
				title = string.Join(" ", tokens.Take(indexOfArtistSplit));
			}
			if (!hasArtist)
			{
				artist = string.Join(" ", tokens.Skip(indexOfArtistSplit + 1).Take(indexOfAlbumSplit - indexOfArtistSplit - 1));
			}
			if (!hasAlbum)
			{
				album = string.Join(" ", tokens.Skip(indexOfAlbumSplit + 1).Take(n - indexOfAlbumSplit));
			}

			return new SongRequest()
			{
				Title = title,
				Artist = artist,
				Album = album,
				Raw = input
			};
		}

		public static bool TryLastIndexOf(string[] tokens, string keyword, out int index, int lowerBound = 0)
		{
			for (index = tokens.Length - 2; index >= lowerBound + 1; index--)
			{
				if (string.Equals(tokens[index], keyword, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			index = tokens.Length;

			return false;
		}
	}
}
