using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.DJ
{

	internal class QueryComparer
	{
		private readonly Regex titleRegex;
		private readonly Regex artistRegex;
		private readonly Regex albumRegex;

		public QueryComparer(Query query)
		{
			titleRegex = query.HasTitle ? GetRegexFromPrefixes(Utility.Tokenize(query.Title)) : null;
			artistRegex = query.HasArtist ? GetRegexFromPrefixes(Utility.Tokenize(query.Artist)) : null;
			albumRegex = query.HasAlbum ? GetRegexFromPrefixes(Utility.Tokenize(query.Album)) : null;
		}

		public bool IsTitleMatch(string name)
		{
			name = Utility.Standardize(name);
			Log.WriteLine($"Testing '{name}' for title...", ConsoleColor.Yellow, Verbosity.silly);
			return titleRegex?.IsMatch(name) ?? false;
		}

		public bool IsArtistMatch(string name)
		{
			name = Utility.Standardize(name);
			Log.WriteLine($"Testing '{name}' for artist...", ConsoleColor.Yellow, Verbosity.silly);
			return artistRegex?.IsMatch(name) ?? false;
		}

		public bool IsAlbumMatch(string name)
		{
			name = Utility.Standardize(name);
			Log.WriteLine($"Testing '{name}' for album...", ConsoleColor.Yellow, Verbosity.silly);
			return albumRegex?.IsMatch(name) ?? false;
		}

		private static Regex GetRegexFromPrefixes(IEnumerable<string> prefixes)
		{
			var terms = prefixes.Select(x => $@"\b{x}[^\s]*");
			return new Regex($"{string.Join(@"\s+([^\s]+\s+)*", terms)}", RegexOptions.IgnoreCase);
		}
	}
}
