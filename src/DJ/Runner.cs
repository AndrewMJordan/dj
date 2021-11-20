using Andtech.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech
{

	internal class Runner
	{
		private readonly Options options;
		private bool Verbose { get; set; }
		private readonly string musicDirectory;

		public Runner(Options options)
		{
			this.options = options;
			Verbose = options.Verbose;

			musicDirectory = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
			musicDirectory = Directory.Exists(musicDirectory) && !string.IsNullOrEmpty(musicDirectory) ? musicDirectory : Environment.CurrentDirectory;
		}

		public async Task List()
		{
			var results = GetRankedAudioFiles();

			if (results.Any())
			{
				foreach (var result in results.OrderByDescending(x => x.Score).Take(5))
				{
					Console.WriteLine($"{result.Score}\t{result.AudioFile.Title}");
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
			var results = GetRankedAudioFiles();

			if (results.Any())
			{
				var best = results.OrderByDescending(x => x.Score).First();

				if (!options.DryRun)
				{
					var player = Environment.GetEnvironmentVariable("PLAYER");
					var process = new AudioPlayerProcess(player)
					{
						Verbose = Verbose,
					};
					process.Play(best.AudioFile);
				}
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

		private IEnumerable<RankResult> GetRankedAudioFiles()
		{
			var searcher = new AudioFileSearcher(musicDirectory);
			var query = Query.Parse(options.Title, options.Artist, options.Album, options.Tokens.ToArray());

			Log("Query is:");
			Log($"  Title: {query.Title}");
			Log($"  Artist: {query.Artist}");
			Log($"  Album: {query.Album}");
			Log($"  Raw: {query.Raw}");
			Log("");

			return searcher.GetRanking(query);
		}
	}
}
