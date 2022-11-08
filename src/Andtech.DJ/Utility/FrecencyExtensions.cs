using Andtech.Common.Frecency;
using System.Linq;

namespace Andtech.DJ.Utility
{

	internal static class FrecencyExtensions
	{

		public static bool Contains(this Cache cache, AudioFile audioFile) => TryGetEntry(cache, audioFile, out var _);

		public static bool TryGetEntry(this Cache cache, AudioFile audioFile, out Entry entry)
		{
			entry = cache.FirstOrDefault(x => x.Key.CompareTo(audioFile.Path) == 0);

			return entry != default;
		}
	}
}
