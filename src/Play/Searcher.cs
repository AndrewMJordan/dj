using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech
{
	class Searcher
	{
		static List<string> ValidExtensions = new List<string>()
		{
			".mp3",
			".mp4",
			".wav",
			".aiff",
			".flac"
		};

		public IEnumerable<string> EnumerateFiles(string root)
		{
			return Directory
				.EnumerateFiles(root, "*", SearchOption.AllDirectories)
				.Where(IsMusicFile);
		}

		static bool IsMusicFile(string path)
		{
			return ValidExtensions.Contains(Path.GetExtension(path));
		}
	}
}
