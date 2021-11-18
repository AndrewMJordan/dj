using CaseExtensions;
using CliWrap;
using CommandLine;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech
{

	class Program
	{

		[Verb("play", isDefault: true, HelpText = "Play the music file.")]
		class PlayOptions
		{
			[Value(0)]
			public IList<string> Tokens { get; set; }

			[Option('l', "list", HelpText = "List search results instead of enqueuing them.")]
			public bool List { get; set; }
		}

		static async Task Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<PlayOptions>(args);
			await result.WithParsedAsync(Play);
		}

		static async Task Play(PlayOptions options)
		{
			if (options.List)
			{
				List(options.Tokens.ToArray());
			}
			else
			{
				var searcher = new MusicFileSearcher();
				var results = searcher.GetRanking(options.Tokens.ToArray());

				if (results.Any())
				{
					var best = results.OrderByDescending(x => x.Score).First();
					var player = Environment.GetEnvironmentVariable("PLAYER");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"Now playing '{best.Term.Transform(To.TitleCase)}'...");
					Console.ResetColor();

					var tokens = Utility.SplitCommand(player);
					var command = tokens.First();

					var arguments = new List<string>(tokens.Skip(1)) { best.Path };

					_ = Cli.Wrap(command)
						.WithArguments(arguments)
						.ExecuteAsync();
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Error.WriteLine($"No matches");
				}
			}
		}

		static void List(string[] args)
		{
			var searcher = new MusicFileSearcher();
			var results = searcher.GetRanking(args);

			if (results.Any())
			{
				foreach (var result in results.OrderByDescending(x => x.Score).Take(5))
				{
					Console.WriteLine($"{result.Score}\t{result.Term.Transform(To.TitleCase)}");
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
