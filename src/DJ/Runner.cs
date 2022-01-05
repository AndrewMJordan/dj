using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech.DJ
{

	internal class Runner
	{
		private readonly Options options;
		private readonly string musicDirectory;
		private readonly Query query;

		public Runner(Options options)
		{
			this.options = options;

			musicDirectory = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
			musicDirectory = Directory.Exists(musicDirectory) && !string.IsNullOrEmpty(musicDirectory) ? musicDirectory : Environment.CurrentDirectory;

			query = Query.Parse(options.Title, options.Artist, options.Album, options.Tokens.ToArray());
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
						WorkingDirectory = musicDirectory
					};
					process.Play(best);
				}

				if (!options.IgnoreMetadata)
				{
					best = AudioFile.Read(best.Path, true);
				}
				if (options.DryRun)
				{
					Log.WriteLine($"[DRY RUN] Now playing {best}...", ConsoleColor.DarkGreen);
				}
				else
				{
					Log.WriteLine($"Now playing {best}...", ConsoleColor.Green);
				}
			}
			else
			{
				Log.WriteLine($"No matches", ConsoleColor.Red, Verbosity.error);
			}
		}
	}
}
