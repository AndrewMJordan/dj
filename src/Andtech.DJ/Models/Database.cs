using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.DJ.Models
{

	internal class Database : IEnumerable<Entry>
	{
		private readonly LinkedList<Entry> list = new LinkedList<Entry>();

		public bool Contains(AudioFile key) => TryGetEntry(key, out var _);

		public bool TryGetEntry(AudioFile key, out Entry entry)
		{
			entry = list.FirstOrDefault(x => x.CompareTo(key) == 0);

			return entry != default;
		}

		public void Add(Entry entry) => list.AddFirst(entry);

		public static Database Read(string path)
		{
			var database = new Database();
			foreach (var line in File.ReadLines(path))
			{
				if (string.IsNullOrEmpty(line))
				{
					continue;
				}

				var entry = Entry.Parse(line);
				database.list.AddLast(entry);
			}

			return database;
		}

		public static void Write(string path, Database database)
		{
			var text = string.Join(Environment.NewLine, database.list.Select(x => x.ToString()));

			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, text);
		}

		public IEnumerator<Entry> GetEnumerator()
		{
			return list.OrderByDescending(GetFreqDate).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)list.OrderByDescending(GetFreqDate)).GetEnumerator();
		}

		static DateTime GetFreqDate(Entry entry) => entry.FreqDate;
	}
}
