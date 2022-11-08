using Andtech.Common;
using Andtech.Common.Text.SentenceExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech.DJ
{

	/// <summary>
	/// Scans a music library for specific content.
	/// </summary>
	/// <remarks>The music library is expected to be organized by Artist > Album > Song.</remarks>
	internal class MusicScanner
	{
		internal struct MatchResult
		{
			public string Path { get; set; }
			public bool Success { get; set; }
			public double NonParenthesizedAccuracy { get; set; }
			public double ParenthesizedAccuracy { get; set; }

		}

		private readonly SongRequest request;

		public MusicScanner(SongRequest request)
		{
			this.request = request;
		}

		public bool TryFindDirectory(string searchRoot, out string path, MusicMetadataField field = MusicMetadataField.Artist, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			var directories = Directory.EnumerateDirectories(searchRoot, "*", searchOption);
			return TryFind(directories, out path, field);
		}

		public bool TryFindFile(string searchRoot, out string path, MusicMetadataField field = MusicMetadataField.Artist, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			var files = AudioFilePath.EnumerateAudioFiles(searchRoot, searchOption);
			return TryFind(files, out path, field);
		}

		bool TryFind(IEnumerable<string> entries, out string path, MusicMetadataField field)
		{
			var matches = entries
				.Select(x => ToResult(x, field))
				.Where(x => x.Success)
				.OrderByDescending(x => x.NonParenthesizedAccuracy)
				.ThenByDescending(x => x.ParenthesizedAccuracy);

			path = matches.FirstOrDefault().Path;
			return !string.IsNullOrEmpty(path);
		}

		MatchResult ToResult(string x, MusicMetadataField field)
		{
			var sentex = new Sentex(Path.GetFileNameWithoutExtension(x));

			TermCollection match;
			switch (field)
			{
				case MusicMetadataField.Song:
					match = sentex.Match(request.Title);
					break;
				case MusicMetadataField.Artist:
					match = sentex.Match(request.Artist);
					break;
				case MusicMetadataField.Album:
					match = sentex.Match(request.Album);
					break;
				default:
					throw new ArgumentException();
			}

			var nonParenthesizedCount = match.Count(x => !x.Word.IsParenthesized);
			var nonParenthesizedMatchCount = match.Count(x => x.Success && !x.Word.IsParenthesized);
			var parenthesizedCount = match.Count(x => x.Word.IsParenthesized);
			var parenthesizedMatchCount = match.Count(x => x.Success && x.Word.IsParenthesized);

			var result = new MatchResult()
			{
				Path = x,
				Success = Macros.Validate(match),
			};

			result.NonParenthesizedAccuracy = nonParenthesizedCount == 0 ? 0.0 : (double)nonParenthesizedMatchCount / nonParenthesizedCount;
			result.ParenthesizedAccuracy = parenthesizedCount == 0 ? 0.0 : (double)parenthesizedMatchCount / parenthesizedCount;

			Log.WriteLine(x, System.ConsoleColor.DarkGray, Verbosity.silly);
			Log.WriteLine("  " + string.Join(", ", $"{result.NonParenthesizedAccuracy:P}", $"{result.ParenthesizedAccuracy:P}"),
				System.ConsoleColor.DarkGray, Verbosity.silly);

			return result;
		}
	}
}
