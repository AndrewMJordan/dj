using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.DJ
{

	class Utility
	{

		/// <summary>
		/// Convert the string to a standard queryable format.
		/// </summary>
		/// <param name="value">The string to standardize.</param>
		/// <returns>The standardized query string.</returns>
		public static string Standardize(string value) => value?.Humanize(LetterCasing.LowerCase);

		/// <summary>
		/// Split the query based on a standard format.
		/// </summary>
		/// <param name="query">The query string.</param>
		/// <returns>The tokens of the query.</returns>
		public static IEnumerable<string> Tokenize(string query) => query.Humanize(LetterCasing.LowerCase).Split(' ');

		/// <summary>
		/// Seperate the tokens of a command line string.
		/// </summary>
		/// <param name="value">The command string to split.</param>
		/// <returns>The tokens of the command string.</returns>
		public static IEnumerable<string> Split(string value)
		{
			var regex = new Regex(@"(?<match>[\w-\.]+)|\""(?<match>[\w\s-\.]*)""|'(?<match>[\w\s-\.]*)'");
			return
				from match in regex.Matches(value)
				select match.Groups["match"].Value;
		}

		/// <summary>
		/// Seperate the tokens of a command line string.
		/// </summary>
		/// <param name="command">The command string to split.</param>
		/// <returns>The tokens of the command string.</returns>
		public static IEnumerable<string> SplitCommand(string command)
		{
			var regex = new Regex(@"(?<match>[\w-\.]+)|\""(?<match>[\w\s-\.]*)""|'(?<match>[\w\s-\.]*)'");
			return
				from match in regex.Matches(command)
				select match.Groups["match"].Value;
		}

		/// <summary>
		/// Run an action in a directory, then switch back.
		/// </summary>
		/// <param name="directory">The directory from which to run in.</param>
		/// <param name="action">The action to perform.</param>
		public static void RunInDirectory(string directory, Action action)
		{
			var tempCurrentDirectory = Environment.CurrentDirectory;
			Environment.CurrentDirectory = directory;
			try
			{
				action();
			}
			finally
			{
				Environment.CurrentDirectory = tempCurrentDirectory;
			}
		}
	}
}
