using CommandLine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Andtech
{

	public class Options
	{
		[Value(0)]
		public IList<string> Tokens { get; set; }

		[Option('l', "list", HelpText = "List search results instead of enqueuing them.")]
		public bool List { get; set; }

		[Option("verbose", HelpText = "Print verbose messages.")]
		public bool Verbose { get; set; }

		[Option('n', "dry-run", HelpText = "Dry run the command.")]
		public bool DryRun { get; set; }

		[Option("title", HelpText = "Filter results by title")]
		public string Title { get; set; }
		[Option("artist", HelpText = "Filter results by artist")]
		public string Artist { get; set; }
		[Option("album", HelpText = "Filter results by album.")]
		public string Album { get; set; }
		[Option("no-metadata", HelpText = "Ignore file metadata.")]
		public bool IgnoreMetadata { get; set; }
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
			var runner = new Runner(options);

			if (options.List)
			{
				await runner.List();
			}
			else
			{
				await runner.Play();
			}
		}
	}
}
