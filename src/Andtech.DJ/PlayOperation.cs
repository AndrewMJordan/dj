using Andtech.Common;
using Andtech.DJ.Models;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech.DJ
{

	internal class PlayOperation
	{
		[Verb("play", isDefault: true, HelpText = "Add file contents to the index.")]
		public class Options : BaseOptions
		{
			[Value(0)]
			public IList<string> Tokens { get; set; }
			[Option("title", HelpText = "Filter results by title")]
			public string Title { get; set; }
			[Option("artist", HelpText = "Filter results by artist")]
			public string Artist { get; set; }
			[Option("album", HelpText = "Filter results by album.")]
			public string Album { get; set; }
			[Option("no-metadata", HelpText = "Never read audio metadata.")]
			public bool IgnoreMetadata { get; set; }
			[Option("no-cache", HelpText = "Do not read from the index.")]
			public bool NoCache { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			var request = SongRequest.Parse(options.Title, options.Artist, options.Album, options.Tokens.ToArray());

			var finder = new MusicFileFinder(request)
			{
				MusicDirectory = Session.Instance.MusicRoot,
				UseMetadata = !options.IgnoreMetadata,
			};

			if (!options.NoCache && SearchIndex(out var audioFile))
			{
				Log.WriteLine($"Found '{audioFile.Path}' in index...", ConsoleColor.Cyan, Verbosity.verbose);
				Play(audioFile);
			}
			else if (finder.TryFindMatch(out audioFile))
			{
				Log.WriteLine($"Found '{audioFile.Path}'...", ConsoleColor.Cyan, Verbosity.verbose);
				Play(audioFile);
			}
			else
			{
				Log.Error.WriteLine($"No matches", ConsoleColor.Red);
			}

			bool SearchIndex(out AudioFile audioFile)
			{
				foreach (var entry in Session.Instance.Index)
				{
					Environment.CurrentDirectory = Session.Instance.MusicRoot;
					var path = entry.Path;
					if (File.Exists(path))
					{
						Log.WriteLine($"Trying '{path}' in index...", ConsoleColor.DarkGray, Verbosity.silly);
						audioFile = AudioFile.Read(path);
						if (MusicFileFinder.IsMatch(request, audioFile))
						{
							return true;
						}
					}
					else
					{
						Log.WriteLine($"'{path}' doesn't exist...", Verbosity.silly);
					}
				}

				audioFile = default;
				return false;
			}

			void Play(AudioFile audioFile)
			{
				if (!options.DryRun)
				{
					var player = Environment.GetEnvironmentVariable("PLAYER");
					var process = new AudioPlayerProcess(player)
					{
						WorkingDirectory = Session.Instance.MusicRoot,
					};
					process.Play(audioFile);
				}

				if (!options.IgnoreMetadata)
				{
					audioFile = AudioFile.Read(audioFile.Path, true);
				}
				if (options.DryRun)
				{
					Log.WriteLine($"[DRY RUN] Now playing {audioFile}...", ConsoleColor.DarkGreen);
				}
				else
				{
					Log.WriteLine($"Now playing {audioFile}...", ConsoleColor.Green);
				}

				if (!options.DryRun)
				{
					audioFile.Path = Path.GetRelativePath(Session.Instance.MusicRoot, audioFile.Path);

					if (Session.Instance.Index.TryGetEntry(audioFile, out var entry))
					{
						entry.CriticalDate = Session.Instance.Frecency.IncreaseScore(entry.CriticalDate);
						entry.PlayCount++;

						Log.WriteLine("Updating query in the index!", Verbosity.verbose);
					}
					else
					{
						entry = new Entry(DateTime.UtcNow, audioFile.Path)
						{
							Path = audioFile.Path,
							PlayCount = 1,
						};

						Session.Instance.Index.Add(entry);

						Log.WriteLine("Query added to the index!", Verbosity.verbose);
					}

					Session.Instance.CommitIndex();
				}
			}
		}
	}
}
