using FuzzySharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech
{

	public class RankResult
	{
		public string Path { get; set; }
		public string Term { get; set; }
		public double Score { get; set; }
	}

	class MusicFileSearcher
	{

		static List<string> ValidExtensions = new List<string>()
		{
			".mp3",
			".mp4",
			".m4a",
			".wav",
			".wma",
			".aac",
			".aiff",
			".flac"
		};

		public IEnumerable<RankResult> GetRanking(params string[] tokens)
		{
			var query = Utility.Standardize(string.Join(" ", tokens));
			var terms = tokens.Select(x => $@"\b{x}[^\s]*");
			var regex = new Regex($"{string.Join(@"\s+([^\s]+\s+)*", terms)}");

			var root = ".";
			var paths = Directory
				.EnumerateFiles(root, "*", SearchOption.AllDirectories)
				.Where(IsMusicFile);

			return paths.Select(ToData).Where(x => regex.IsMatch(x.Term));

			RankResult ToData(string path)
			{
				var filename = Path.GetFileNameWithoutExtension(path);
				var term = Utility.Standardize(filename);

				return new RankResult { Path = path, Term = term, Score = Fuzz.Ratio(query, term, FuzzySharp.PreProcess.PreprocessMode.Full) };
			}
		}

		static bool IsMusicFile(string path)
		{
			return ValidExtensions.Contains(Path.GetExtension(path));
		}
	}
}
