using Andtech.Common;
using Andtech.DJ.Models;
using Andtech.DJ.Utility;
using CommandLine;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			var result = Parser.Default.ParseArguments<BaseOptions, PlayOperation.Options, ListOperation.Options>(args);
			result.WithParsed<BaseOptions>(PreParse);
			await result.WithParsedAsync<PlayOperation.Options>(PlayOperation.OnParseAsync);
			await result.WithParsedAsync<ListOperation.Options>(ListOperation.OnParseAsync);
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
				Session.Instance.Index = Database.Read(Session.Instance.IndexPath);
			}
			else
			{
				Session.Instance.Index = new Database();
			}
		}
	}
}
