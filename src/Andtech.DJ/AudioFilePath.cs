using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech.DJ
{

	public static class AudioFilePath
	{
		public delegate bool MatchPredicate(string expected);

		static readonly List<string> ValidExtensions = new List<string>()
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

		public static bool IsMusicFile(string path) => ValidExtensions.Contains(Path.GetExtension(path));

		public static bool GetDirectory(string searchRoot, out string matchPath, MatchPredicate predicate = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			var directories = Directory.EnumerateDirectories(searchRoot, "*", SearchOption.TopDirectoryOnly);
			foreach (var directory in directories)
			{
				var name = Path.GetFileNameWithoutExtension(directory);
				name = Utility.Standardize(name);
				if (predicate?.Invoke(name) ?? true)
				{
					matchPath = directory;
					return true;
				}
			}

			if (searchOption == SearchOption.AllDirectories)
			{
				var children = Directory.EnumerateDirectories(searchRoot, "*", SearchOption.TopDirectoryOnly);
				foreach (var child in children)
				{
					if (GetDirectory(child, out matchPath, predicate, searchOption))
					{
						return true;
					}
				}
			}

			matchPath = default;
			return false;
		}

		public static bool GetFile(string searchRoot, out string matchPath, MatchPredicate predicate = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			var files = Directory.EnumerateFiles(searchRoot, "*", SearchOption.TopDirectoryOnly)
				.Where(IsMusicFile);
			foreach (var file in files)
			{
				var name = Path.GetFileNameWithoutExtension(file);
				if (predicate?.Invoke(name) ?? true)
				{
					matchPath = file;
					return true;
				}
			}

			if (searchOption == SearchOption.AllDirectories)
			{
				var children = Directory.EnumerateDirectories(searchRoot, "*", SearchOption.TopDirectoryOnly);
				foreach (var child in children)
				{
					if (GetFile(child, out matchPath, predicate, searchOption))
					{
						return true;
					}
				}
			}

			matchPath = default;
			return false;
		}
	}
}
