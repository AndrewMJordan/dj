using Andtech.Models;
using CaseExtensions;
using CliWrap;
using CommandLine;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
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
				var musicDir = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
				musicDir = Directory.Exists(musicDir) ? musicDir : Environment.CurrentDirectory;

				var searcher = new AudioFileSearcher(musicDir);
				var results = searcher.GetRanking(options.Tokens.ToArray());

				if (results.Any())
				{
					var best = results.OrderByDescending(x => x.Score).First();

					var player = Environment.GetEnvironmentVariable("PLAYER");
					var process = new AudioPlayerProcess(player);

					var audioFile = new AudioFile()
                    {
						Path = best.Path,
						Title = best.Term.Humanize(LetterCasing.Title),
						Artist = "Unknown Artist"
                    };
                    process.Play(audioFile);
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
			var searcher = new AudioFileSearcher();
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
