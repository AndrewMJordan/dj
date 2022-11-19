using Andtech.Common;
using Andtech.Common.Text.SentenceExpressions;
using System.IO;
using System.Linq;

namespace Andtech.DJ
{

	internal class FileSystemScanner
	{
		public bool IsValid { get; private set; }
		public int Depth { get; private set; }
		private readonly string searchRoot;

		public FileSystemScanner(string searchRoot)
		{
			this.searchRoot = searchRoot;
			IsValid = true;
		}

		public FileSystemScanner FindDirectory(string query)
		{
			if (!IsValid)
			{
				return this;
			}

			var matches = Directory.EnumerateDirectories(searchRoot, "*", SearchOption.AllDirectories)
				.Select(x => PathToResult(query, x))
				.Where(x => x.AnySuccess)
				.OrderByDescending(x => x.StrictSuccess)
				.OrderByDescending(x => x.ParenthesizedAccuracy)
				.ThenByDescending(x => x.NonParenthesizedAccuracy)
				.ToArray();

			var isStrictMatch = matches.Any(x => x.StrictSuccess);
			var nextSearchRoot = matches.Length > 0 ? matches[0].Path : searchRoot;
			return new FileSystemScanner(nextSearchRoot)
			{
				IsValid = isStrictMatch,
				Depth = Depth + 1,
			};
		}

		public bool File(string query, out string path)
		{
			if (!IsValid)
			{
				path = null;
				return false;
			}

			var matches = AudioFilePath.EnumerateAudioFiles(searchRoot, SearchOption.AllDirectories)
				.Select(x => PathToResult(query, x))
				.Where(x => x.AnySuccess)
				.OrderByDescending(x => x.StrictSuccess)
				.OrderByDescending(x => x.ParenthesizedAccuracy)
				.ThenByDescending(x => x.NonParenthesizedAccuracy)
				.ToArray();

			var isStrictMatch = matches.Any(x => x.StrictSuccess);
			path = matches.Length > 0 ? matches[0].Path : null;
			return isStrictMatch;
		}

		static ScanResult PathToResult(string query, string path)
		{
			var sentex = new Sentex(Path.GetFileNameWithoutExtension(path));
			var match = sentex.Match(query);

			var nonParenthesizedCount = match.Count(x => !x.Word.IsParenthesized);
			var nonParenthesizedMatchCount = match.Count(x => x.Success && !x.Word.IsParenthesized);
			var parenthesizedCount = match.Count(x => x.Word.IsParenthesized);
			var parenthesizedMatchCount = match.Count(x => x.Success && x.Word.IsParenthesized);

			var result = new ScanResult()
			{
				Path = path,
				AnySuccess = Macros.IsAnySuccess(match),
				StrictSuccess = Macros.IsStrictSuccess(match),
			};

			result.NonParenthesizedAccuracy = nonParenthesizedCount == 0 ? 0.0 : (double)nonParenthesizedMatchCount / nonParenthesizedCount;
			result.ParenthesizedAccuracy = parenthesizedCount == 0 ? 0.0 : (double)parenthesizedMatchCount / parenthesizedCount;

			Log.WriteLine(path, System.ConsoleColor.DarkGray, Verbosity.silly);
			Log.WriteLine("  " + string.Join(", ", $"{nonParenthesizedMatchCount}/{nonParenthesizedCount}", $"{parenthesizedMatchCount}/{ parenthesizedCount}"),
				System.ConsoleColor.DarkGray, Verbosity.silly);

			return result;
		}

		internal struct ScanResult
		{
			public string Path { get; set; }
			public bool AnySuccess { get; set; }
			public bool StrictSuccess { get; set; }
			public double NonParenthesizedAccuracy { get; set; }
			public double ParenthesizedAccuracy { get; set; }
		}
	}
}
