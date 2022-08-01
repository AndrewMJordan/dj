using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech.DJ
{

	/// <summary>
	/// Access to IO operations on audio files.
	/// </summary>
	/// <seealso cref="System.IO.Path"/>
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

		public static IEnumerable<string> EnumerateAudioFiles(string searchRoot, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			return Directory.EnumerateFiles(searchRoot, "*", SearchOption.TopDirectoryOnly)
				.Where(IsMusicFile);
		}
	}
}
