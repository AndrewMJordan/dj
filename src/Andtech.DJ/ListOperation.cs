using CommandLine;
using Humanizer;
using System;
using System.Threading.Tasks;

namespace Andtech.DJ
{

	internal class ListOperation
	{

		[Verb("list", aliases: new string[] { "ls" }, HelpText = "List database status.")]
		public class Options : BaseOptions
		{
			[Option("offset", HelpText = "Offset scores a number of days.")]
			public double Offset { get; set; }
			[Option('l', "long", HelpText = "Show extended details")]
			public bool Long { get; set; }
			[Option('h', "humanize", HelpText = "Show humanized output")]
			public bool Humanize { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			foreach (var entry in Session.Instance.Index)
			{
				var freqDate = entry.CriticalDate.AddDays(options.Offset);
				var score = Session.Instance.Frecency.Decode(freqDate);

				var scoreString = string.Format(score < 0.95 ? "{0:0.0}" : "{0:0}", score * 100.0);

				if (options.Long)
				{
					if (options.Humanize)
					{
						Console.WriteLine($"{scoreString}	{entry.PlayCount}	{freqDate.Humanize()}		{entry.Key}");
					}
					else
					{
						var tau = Math.Abs((DateTime.UtcNow - freqDate).TotalDays);
						Console.WriteLine($"{scoreString}	{entry.PlayCount}	{tau:0.00}		{entry.Key}");
					}
				}
				else
				{
					Console.WriteLine($"{scoreString}	{entry.Key}");
				}
			}
		}
	}
}
