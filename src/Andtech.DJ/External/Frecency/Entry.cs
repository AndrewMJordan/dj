using System;

namespace Andtech.Common.Frecency
{

	public class Entry
	{
		public DateTime CriticalDate { get; set; }
		public string Key { get; set; }
		public int PlayCount { get; set; }

		private Entry()
		{

		}

		public Entry(DateTime dateTime, string key)
		{
			CriticalDate = dateTime;
			Key = key;
		}

		public static Entry Parse(string text)
		{
			var tokens = text.Split(',');
			var entry = new Entry()
			{
				PlayCount = int.Parse(tokens[0]),
				CriticalDate = DateTime.Parse(tokens[1]).ToUniversalTime(),
				Key = tokens[2],
			};

			return entry;
		}

		public override string ToString() => string.Join(",", PlayCount, CriticalDate.ToString("O"), Key);
	}
}
