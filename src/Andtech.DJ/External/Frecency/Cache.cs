using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech.Common.Frecency
{

	public class Cache : IEnumerable<Entry>
	{
		private readonly LinkedList<Entry> list = new LinkedList<Entry>();

		public int Count => list.Count;

		public void Add(Entry entry) => list.AddFirst(entry);

		public bool Remove(Entry entry) => list.Remove(entry);

		public static Cache Read(string path)
		{
			var cache = new Cache();
			foreach (var line in File.ReadLines(path))
			{
				if (string.IsNullOrEmpty(line))
				{
					continue;
				}

				var entry = Entry.Parse(line);
				cache.list.AddLast(entry);
			}

			return cache;
		}

		public static void Write(string path, Cache database)
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

		static DateTime GetFreqDate(Entry entry) => entry.CriticalDate;
	}
}
