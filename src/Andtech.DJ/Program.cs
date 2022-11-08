using Andtech.Common;
using Andtech.Common.Frecency;
using CommandLine;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Andtech.DJ
{

	public class BaseOptions
	{
		[Option("verbosity", HelpText = "Sets the verbosity level of the command.")]
		public Verbosity Verbosity { get; set; }
		[Option('v', "verbose", HelpText = "Run the command with verbose output. (Same as running with --verbosity verbose)")]
		public bool Verbose { get; set; }
		[Option('n', "dry-run", HelpText = "Dry run the command.")]
		public bool DryRun { get; set; }
	}

	class Program
	{

		static async Task Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<BaseOptions, PlayOperation.Options, ListOperation.Options, PruneOperation.Options>(args);
			result.WithParsed<BaseOptions>(PreParse);
			await result.WithParsedAsync<PlayOperation.Options>(PlayOperation.OnParseAsync);
			await result.WithParsedAsync<ListOperation.Options>(ListOperation.OnParseAsync);
			await result.WithParsedAsync<PruneOperation.Options>(PruneOperation.OnParseAsync);
		}

		public static void PreParse(BaseOptions options)
		{
			Log.Verbosity = options.Verbose ? Verbosity.verbose : options.Verbosity;

			var musicRoot = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
			musicRoot = Directory.Exists(musicRoot) && !string.IsNullOrEmpty(musicRoot) ? musicRoot : Environment.CurrentDirectory;
			Session.Instance = new Session()
			{
				Frecency = new Frecency(7.0),
				MusicRoot = musicRoot,
			};

			var indexRoot = musicRoot;
			var indexPath = Path.Combine(indexRoot, "dj.index");

			Session.Instance.IndexPath = indexPath;
			if (File.Exists(Session.Instance.IndexPath))
			{
				Session.Instance.Index = Cache.Read(Session.Instance.IndexPath);
			}
			else
			{
				Session.Instance.Index = new Cache();
			}
		}
	}
}
