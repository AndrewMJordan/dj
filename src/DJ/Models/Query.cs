using System;
using System.Linq;

namespace Andtech.Models
{

	internal class Query
	{
		public string Title { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public string Raw { get; set; }

		public static Query Parse(string title, string artist, string album, params string[] tokens)
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

			return new Query()
			{
				Title = Utility.Standardize(title),
				Artist = Utility.Standardize(artist),
				Album = Utility.Standardize(album),
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
