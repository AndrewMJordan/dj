using Andtech.Models;
using CaseExtensions;
using CliWrap;
using CommandLine;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
