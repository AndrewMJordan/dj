using Andtech.Common;
using Andtech.Common.Frecency;
using Andtech.DJ.Utility;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			Log.WriteLine($"Title query is: '{request.Title}'", Verbosity.diagnostic);
			Log.WriteLine($"Artist query is: '{request.Artist}'", Verbosity.diagnostic);
			Log.WriteLine($"Album query is: '{request.Album}'", Verbosity.diagnostic);

			AudioFile audioFile = null;
			if (!options.NoCache)
			{
				var searcher = new CacheSearcher(request)
				{
					UseMetadata = !options.IgnoreMetadata,
				};

				var sw = Stopwatch.StartNew();
				var success = searcher.Search(out audioFile);
				sw.Stop();



				if (success)
				{
					Log.WriteLine($"Found '{audioFile.Title}' in cache ({sw.ElapsedMilliseconds} ms)...", ConsoleColor.Cyan, Verbosity.verbose);
				}
			}
			if (audioFile is null)
			{
				var searcher = new FileSystemSearcher(request)
				{
					MusicDirectory = Session.Instance.MusicRoot,
					UseMetadata = !options.IgnoreMetadata,
				};

				var sw = Stopwatch.StartNew();
				var success = searcher.Search(out audioFile);
				sw.Stop();

				if (success)
				{
					Log.WriteLine($"Found '{audioFile.Title}' ({sw.ElapsedMilliseconds} ms)...", ConsoleColor.Cyan, Verbosity.verbose);
				}
			}

			if (audioFile is null)
			{
				Log.Error.WriteLine($"No matches", ConsoleColor.Red);
			}
			else
			{
				Play(audioFile);
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
							Key = audioFile.Path,
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
