using Andtech.Models;
using FuzzySharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech
{

	internal class RankResult
	{
		public string Term { get; set; }
		public double Score { get; set; }
		public AudioFile AudioFile { get; set; }
	}

	class AudioFileSearcher
	{
		public SearchReport Report { get; private set; }

		private readonly string searchRoot;
		private readonly bool useMetadata;

		public enum SearchReport
		{
			Undefined,
			SearchedByFilePath,
			SearchedByMetadata
		}

		public AudioFileSearcher(string searchRoot = ".", bool useMetadata = true)
		{
			this.searchRoot = searchRoot;
			this.useMetadata = useMetadata;
		}

		static List<string> ValidExtensions = new List<string>()
		{
			".aa",
			".aax",
			".aac",
			".aiff",
			".ape",
			".dsf",
			".flac",
			".m4a",
			".m4b",
			".m4p",
			".mp3",
			".mpc",
			".mpp",
			".ogg",
			".oga",
			".wav",
			".wma",
			".wv",
			".webm",
		};

		public bool TryGetExact(Query query, out AudioFile audioFile)
		{
			if (!File.Exists(query.Raw))
			{
				audioFile = default;
				return false;
			}

			audioFile = AudioFile.Read(query.Raw, useMetadata);
			return true;
		}

		public IEnumerable<AudioFile> EnumerateFiles(Query query)
		{
			var paths = Directory
				.EnumerateFiles(searchRoot, "*", SearchOption.AllDirectories)
				.Where(IsMusicFile);

			var comparer = new AudioFileComparer(query);
			var prepass = paths
				.Where(x => comparer.IsMatch(Path.GetRelativePath(searchRoot, x)))
				.ToList();

			if (prepass.Any())
			{
				var prepassAudioFiles = prepass
					.Select(x => AudioFile.Read(x, useMetadata))
					.Where(comparer.IsMatch)
					.ToList();

				if (prepassAudioFiles.Any())
				{
					Report = SearchReport.SearchedByFilePath;
					return prepassAudioFiles;
				}
			}

			Report = SearchReport.SearchedByMetadata;
			return paths
				.Select(x => AudioFile.Read(x, useMetadata))
				.Where(comparer.IsMatch);
		}

		static bool IsMusicFile(string path)
		{
			return ValidExtensions.Contains(Path.GetExtension(path));
		}
	}
}
