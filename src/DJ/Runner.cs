using Andtech.Models;
using CommandLine;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech
{

    internal class Runner
    {
		private readonly Options options;
        private bool Verbose { get; set; }

		public Runner(Options options)
        {
			this.options = options;
			Verbose = options.Verbose;
        }

		public async Task List()
		{
			var musicDir = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
			musicDir = Directory.Exists(musicDir) ? musicDir : Environment.CurrentDirectory;
			var searcher = new AudioFileSearcher(musicDir);
			var query = Utility.Standardize(string.Join(" ", options.Tokens));
			var results = searcher.GetRanking(options.Tokens.ToArray());

			Log("List mode...");

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

		public async Task Play()
		{
			var musicDir = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
			musicDir = Directory.Exists(musicDir) ? musicDir : Environment.CurrentDirectory;
			var searcher = new AudioFileSearcher(musicDir);

			var query = Utility.Standardize(string.Join(" ", options.Tokens));

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

		void Log(object message, bool always = false)
        {
            if (Verbose || always)
            {
				Console.WriteLine(message);
            }
        }
    }
}
