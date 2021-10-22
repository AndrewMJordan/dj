using CaseExtensions;
using CliWrap;
using F23.StringSimilarity;
using FuzzySharp;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Andtech
{

	class Program
	{

		class FileData
		{
			public string Path { get; set; }
			public string Term { get; set; }
			public double Score { get; set; }
		}

		static async Task Main(string[] args)
		{
			var root = ".";
			var query = Standardize(string.Join(" ", args));

			var l = new MetricLCS();

			var terms = args.Select(x => $@"\b{x}[^\s]*");
			var regex = new Regex($"{string.Join(@"\s+([^\s]+\s+)*", terms)}");
			Console.WriteLine(regex);

			var searcher = new Searcher();
			var files = searcher.EnumerateFiles(root);
			var datas = files
				.Select(ToFileData);
			Console.WriteLine($"'{query}'");

			FileData ToFileData(string path)
			{
				var filename = Path.GetFileNameWithoutExtension(path);
				var term = Standardize(filename);

				return new FileData { Path = path, Term = term, Score = Fuzz.PartialTokenSetRatio(query, term, FuzzySharp.PreProcess.PreprocessMode.Full) };
			}

			datas = datas.Where(x => regex.IsMatch(x.Term));

			var top = datas.OrderByDescending(x => x.Score).Take(6);

			foreach (var element in datas.OrderByDescending(x => x.Score).Take(6))
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"{element.Term} ({element.Score})");
			}

			if (top.Any())
			{
				var best = top.First();
				Console.ForegroundColor = ConsoleColor.Green;
				var player = Environment.GetEnvironmentVariable("PLAYER");
				Console.WriteLine($"{player} {best.Path}");

				return;
				await Cli.Wrap(player)
					.WithArguments(best.Path)
					.ExecuteAsync();
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine($"No matches");
			}
		}

		static string Standardize(string x)
		{
			x = x.ToKebabCase();
			x = x.Replace("-", " ");
			return x;
		}
	}
}
