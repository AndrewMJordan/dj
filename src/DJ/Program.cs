using CaseExtensions;
using CliWrap;
using CommandLine;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech
{

	class Program
	{

		[Verb("play", isDefault: true, HelpText = "Play the music file.")]
		class PlayOptions
		{
		}

		[Verb("list", HelpText = "List the files ranking given a query.")]
		class ListOptions
		{
		}

		private static string[] Args;
		private static readonly string[] Verbs = new string[] { "play", "list" };

		static async Task Main(string[] args)
		{
			Args = args.Except(Verbs).ToArray();
			var result = Parser.Default.ParseArguments<PlayOptions, ListOptions>(args);
			await result.WithParsedAsync<PlayOptions>(Play);
			result.WithParsed<ListOptions>(List);
		}

		static async Task Play(PlayOptions options)
		{
			var searcher = new MusicFileSearcher();
			var results = searcher.GetRanking(Args);

			if (results.Any())
			{
				var best = results.OrderByDescending(x => x.Score).First();
				var player = Environment.GetEnvironmentVariable("PLAYER");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"Now playing: '{best.Path.ToPascalCase()}'");
				Console.ResetColor();

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

		static void List(ListOptions options)
		{
			var searcher = new MusicFileSearcher();
			var results = searcher.GetRanking(Args);

			if (results.Any())
			{
				foreach (var result in results.OrderByDescending(x => x.Score).Take(5))
				{
					Console.WriteLine($"{result.Score}\t{result.Term}");
				}
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine($"No matches");
			}
		}
	}
}
