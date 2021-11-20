using Humanizer;

namespace Andtech.Models
{

	internal class AudioFile
	{
		public string Path { get; set; }
		public string Title { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }

		public static AudioFile Read(string path)
		{
			var tfile = TagLib.File.Create(path);

			return new AudioFile()
			{
				Path = path,
				Title = tfile.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(path).Humanize(LetterCasing.Title),
				Album = tfile.Tag.Album,
				Artist = tfile.Tag.FirstPerformer
			};
		}

		public override string ToString()
		{
			var message = $"'{Title}'";
			if (!string.IsNullOrWhiteSpace(Artist))
			{
				message += $" by '{Artist}'";
			}
			if (!string.IsNullOrWhiteSpace(Album))
			{
				message += $" ({Album})";
			}

			return message;
		}
	}
}
