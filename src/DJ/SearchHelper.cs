using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.DJ
{

	public class SearchHelper
	{
		public string MusicDirectory { get; set; } = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
		public bool UseMetadata { get; set; } = true;

		public bool TryFindMatch(string query, out AudioFile audioFile) => TryFindMatch(Query.Parse(query), out audioFile);

		public bool TryFindMatch(Query query, out AudioFile audioFile)
		{
			audioFile = Search(query)
				.OrderByDescending(x => x.Score)
				.FirstOrDefault()
				.AudioFile;

			return audioFile != null;
		}

		public IEnumerable<RankResult> Search(Query query)
		{
			var searcher = new AudioFileSearcher(MusicDirectory, UseMetadata);
			if (searcher.TryGetExact(query, out var audioFile))
			{
				var result = new RankResult
				{
					AudioFile = audioFile,
					Score = 100,
					Term = string.Empty,
				};
				return Enumerable.Repeat(result, 1);
			}

			return searcher
				.EnumerateFiles(query)
				.Select(ToFileRank)
				.OrderByDescending(x => x.Score);
			
			RankResult ToFileRank(AudioFile audioFile)
			{
				var expected = Utility.Standardize(query.Title);
				var actual = Utility.Standardize(audioFile.Title);
				var score = Fuzz.Ratio(expected, actual, FuzzySharp.PreProcess.PreprocessMode.Full);

				return new RankResult { AudioFile = audioFile, Term = actual, Score = score };
			}
		}
	}
}
