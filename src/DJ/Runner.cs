﻿using ConsoleTables;
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
						WorkingDirectory = musicDirectory
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

		void Log(object message, ConsoleColor foregroundColor = ConsoleColor.Yellow, bool always = false)
		{
			if (Verbose || always)
			{
				Console.ForegroundColor = foregroundColor;
				Console.WriteLine(message);
				Console.ResetColor();
			}
		}

		private IEnumerable<RankResult> GetRankedAudioFiles()
		{
			var searcher = new AudioFileSearcher(musicDirectory, !options.IgnoreMetadata);
			var query = Query.Parse(options.Title, options.Artist, options.Album, options.Tokens.ToArray());
			if (searcher.TryGetExact(query, out var audioFile))
			{
				var result = new RankResult
				{
					AudioFile = audioFile,
					Score = 100,
					Term = string.Empty,
				};
				return Enumerable.Repeat(result, 1);
			}

			Log("Query is:");
			Log($"  Title: {query.Title}");
			Log($"  Artist: {query.Artist}");
			Log($"  Album: {query.Album}");
			Log($"  Raw: {query.Raw}");
			Log("");

			var results = searcher
				.EnumerateFiles(query)
				.Select(ToFileRank)
				.ToList();

			if (searcher.Report == AudioFileSearcher.SearchReport.SearchedByFilePath)
			{
				Log("Fast prepass succeeded! Results were found by analyzing the filepath", ConsoleColor.Cyan);
			}

			return results;

			RankResult ToFileRank(AudioFile audioFile)
			{
				var expected = Utility.Standardize(query.Title);
				var actual = Utility.Standardize(audioFile.Title);
				var score = Fuzz.Ratio(expected, actual, FuzzySharp.PreProcess.PreprocessMode.Full);

				return new RankResult { AudioFile = audioFile, Term = actual, Score = score };
			}
		}
	}
}
