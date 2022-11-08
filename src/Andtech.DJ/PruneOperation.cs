using Andtech.Common;
using Andtech.Common.Frecency;
using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Andtech.DJ
{

	internal class PruneOperation
	{

		[Verb("prune", HelpText = "Prune the database of missing items.")]
		public class Options : BaseOptions
		{
		}

		public static async Task OnParseAsync(Options options)
		{
			Environment.CurrentDirectory = Session.Instance.MusicRoot;

			var toRemove = new HashSet<Entry>(Session.Instance.Index.Count);
			foreach (var entry in Session.Instance.Index)
			{
				if (!File.Exists(entry.Key))
				{
					toRemove.Add(entry);
				}
			}

			foreach (var entry in toRemove)
			{
				DryRun.TryExecute(Prune, $"Prune {entry.Key}");

				void Prune()
				{
					Session.Instance.Index.Remove(entry);
				}
			}

			Session.Instance.CommitIndex();
			Log.WriteLine($"{toRemove.Count} entries pruned!", ConsoleColor.Green);
		}
	}
}
