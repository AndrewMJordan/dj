using Andtech.Common;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Andtech.DJ
{

	public class Options
	{
		[Value(0)]
		public IList<string> Tokens { get; set; }
		[Option("title", HelpText = "Filter results by title")]
		public string Title { get; set; }
		[Option("artist", HelpText = "Filter results by artist")]
		public string Artist { get; set; }
		[Option("album", HelpText = "Filter results by album.")]
		public string Album { get; set; }
		[Option("no-metadata", HelpText = "Never read audio metadata.")]
		public bool IgnoreMetadata { get; set; }

		[Option('n', "dry-run", HelpText = "Dry run the command.")]
		public bool DryRun { get; set; }

		[Option("verbosity", HelpText = "Sets the verbosity level of the command.", Default = "info")]
		public string Verbosity { get; set; }
		[Option('v', "verbose", HelpText = "Run the command with verbose output. (Same as running with --verbosity verbose)")]
		public bool Verbose { get; set; }
	}

	class Program
	{

		static async Task Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<Options>(args);
			await result.WithParsedAsync(OnParse);
		}

		public static async Task OnParse(Options options)
		{
			Log.Verbosity = options.Verbose ? Verbosity.verbose : Enum.Parse<Verbosity>(options.Verbosity);
			var runner = new Runner(options);
			await runner.Play();
		}
	}
}
