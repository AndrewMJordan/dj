using Andtech.Common;
using Andtech.Common.Text.SentenceExpressions;
using System;
using System.IO;

namespace Andtech.DJ.Utility
{

	internal class CacheSearcher
	{
		public bool UseMetadata { get; set; } = true;

		private readonly SongRequest request;

		public CacheSearcher(SongRequest request)
		{
			this.request = request;
		}

		public bool Search(out AudioFile audioFile)
		{
			foreach (var entry in Session.Instance.Index)
			{
				Environment.CurrentDirectory = Session.Instance.MusicRoot;
				var path = entry.Key;
				if (File.Exists(path))
				{
					Log.WriteLine($"Trying '{path}' in index...", ConsoleColor.DarkGray, Verbosity.silly);
					audioFile = AudioFile.Read(path, UseMetadata);
					if (IsMatch(request, audioFile))
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

		bool IsMatch(SongRequest request, AudioFile audioFile)
		{
			if (!string.IsNullOrEmpty(request.Title))
			{
				var sentex = new Sentex(audioFile.Title);
				var match = sentex.Match(request.Title);

				if (!Macros.Validate(match))
				{
					return false;
				}
			}
			if (!string.IsNullOrEmpty(request.Artist))
			{
				var sentex = new Sentex(audioFile.Artist);
				var match = sentex.Match(request.Artist);

				if (!Macros.Validate(match))
				{
					return false;
				}
			}
			if (!string.IsNullOrEmpty(request.Album))
			{
				var sentex = new Sentex(audioFile.Album);
				var match = sentex.Match(request.Album);

				if (!Macros.Validate(match))
				{
					return false;
				}
			}

			return true;
		}
	}
}
