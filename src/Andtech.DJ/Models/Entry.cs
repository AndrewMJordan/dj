using System;

namespace Andtech.DJ.Models
{

	internal class Entry : IComparable<AudioFile>
	{
		public DateTime CriticalDate { get; set; }
		public string Path { get; set; }
		public int PlayCount { get; set; }

		private Entry()
		{

		}

		public Entry(DateTime dateTime, string path)
		{
			CriticalDate = dateTime;
			Path = path;
		}

		public static Entry Parse(string text)
		{
			var tokens = text.Split(',');
			var entry = new Entry()
			{
				PlayCount = int.Parse(tokens[0]),
				CriticalDate = DateTime.Parse(tokens[1]).ToUniversalTime(),
				Path = tokens[2],
			};

			return entry;
		}

		public override string ToString() => string.Join(",", PlayCount, CriticalDate.ToString("O"), Path);

		public int CompareTo(AudioFile other) => Path.CompareTo(other.Path);
	}
}
