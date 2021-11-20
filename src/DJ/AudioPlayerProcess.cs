using Andtech.Models;
using CliWrap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech
{

	internal class AudioPlayerProcess
	{
		public bool Verbose { get; set; }
		public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;

		private readonly string command;

		public AudioPlayerProcess(string command)
		{
			this.command = command;
		}

		public void Play(AudioFile audioFile)
		{
			var tokens = Utility.SplitCommand(command);
			var executable = tokens.First();

			var arguments = new List<string>(tokens.Skip(1)) { Path.GetRelativePath(WorkingDirectory, audioFile.Path) };

			var message = $"Now playing '{audioFile.Title}'";
			if (!string.IsNullOrWhiteSpace(audioFile.Artist))
			{
				message += $" by '{audioFile.Artist}'";
			}

			if (Verbose)
			{
				Console.WriteLine($"{executable} {string.Join(" ", arguments)}");
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"{message}...");
			Console.ResetColor();

			_ = Cli.Wrap(executable)
				.WithWorkingDirectory(WorkingDirectory)
				.WithArguments(arguments)
				.ExecuteAsync();

		}
	}
}
