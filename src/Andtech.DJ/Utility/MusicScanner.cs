using Andtech.Common;
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
			public Sentence Sentence { get; set; }
			public int TotalMatchCount { get; set; }
			public int NonParenthesizedMatchCount { get; set; }
			public int ParenthesizedMatchCount { get; set; }
			public double Accuracy => (double)TotalMatchCount / Sentence.Words.Count();
		}

		private readonly SentenceComparer songComparer;
		private readonly SentenceComparer artistComparer;
		private readonly SentenceComparer albumComparer;

		public MusicScanner(SongRequest request)
		{
			var titleSentence = Sentence.Parse(request.Title);
			var artistSentence = Sentence.Parse(request.Artist);
			var albumSentence = Sentence.Parse(request.Album);

			songComparer = new SentenceComparer(titleSentence);
			artistComparer = new SentenceComparer(artistSentence);
			albumComparer = new SentenceComparer(albumSentence);
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
				.Where(x => x.TotalMatchCount > 0)
				.OrderByDescending(x => x.NonParenthesizedMatchCount)
				.ThenByDescending(x => x.ParenthesizedMatchCount)
				.ThenByDescending(x => x.Accuracy);

			path = matches.FirstOrDefault().Path;
			return !string.IsNullOrEmpty(path);
		}

		MatchResult ToResult(string x, MusicMetadataField field)
		{
			var comparer = GetComparer(field);

			var sentence = Sentence.Parse(Path.GetFileNameWithoutExtension(x));
			var result = new MatchResult()
			{
				Path = x,
				Sentence = sentence,
				TotalMatchCount = comparer.CountMatches(sentence.Words),
				NonParenthesizedMatchCount = comparer.CountMatches(sentence.NonParenthesizedWords),
				ParenthesizedMatchCount = comparer.CountMatches(sentence.ParenthesizedWords),
			};

				Log.WriteLine($"{string.Join(" ", sentence.NonParenthesizedWords)} ({string.Join(",", sentence.ParenthesizedWords)})\t| {result.NonParenthesizedMatchCount}/{sentence.Words.Count()}\t| {result.ParenthesizedMatchCount}/{sentence.Words.Count()}", System.ConsoleColor.Gray, Verbosity.silly);

			return result;
		}

		SentenceComparer GetComparer(MusicMetadataField x)
		{
			switch (x)
			{
				case MusicMetadataField.Song:
					return songComparer;
				case MusicMetadataField.Artist:
					return artistComparer;
				case MusicMetadataField.Album:
					return albumComparer;
			}

			return null;
		}
	}
}
