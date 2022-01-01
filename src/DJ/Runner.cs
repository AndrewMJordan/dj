using ConsoleTables;
using FuzzySharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech.DJ
{

	internal class Runner
	{
		private readonly Options options;
		private bool Verbose { get; set; }
		private readonly string musicDirectory;
		private readonly Query query;

		public Runner(Options options)
		{
			this.options = options;
			Verbose = options.Verbose;

			musicDirectory = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
			musicDirectory = Directory.Exists(musicDirectory) && !string.IsNullOrEmpty(musicDirectory) ? musicDirectory : Environment.CurrentDirectory;

			query = Query.Parse(options.Title, options.Artist, options.Album, options.Tokens.ToArray());
		}

		public async Task List()
		{
			var searchHelper = new SearchHelper
			{
				MusicDirectory = musicDirectory,
				UseMetadata = !options.IgnoreMetadata
			};
			var results = searchHelper.Search(query);

			if (results.Any())
			{
				var table = new ConsoleTable("Score", "Title", "Artist", "Album");
				foreach (var result in results.OrderByDescending(x => x.Score).Take(5))
				{
					table.AddRow(result.Score, result.AudioFile.Title, result.AudioFile.Artist, result.AudioFile.Album);
				}

				table.Write(Format.Minimal);
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine($"No matches");
			}
		}

		public async Task Play()
		{
			var searchHelper = new SearchHelper
			{
				MusicDirectory = musicDirectory,
				UseMetadata = !options.IgnoreMetadata
			};

			if (searchHelper.TryFindMatch(query, out var best))
			{
				if (!options.DryRun)
				{
					var player = Environment.GetEnvironmentVariable("PLAYER");
					var process = new AudioPlayerProcess(player)
					{
						Verbose = Verbose,
						WorkingDirectory = musicDirectory
					};
					process.Play(best);
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
