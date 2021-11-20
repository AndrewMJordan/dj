using Andtech.Models;
using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Andtech
{

	internal class AudioPlayerProcess
	{
		private readonly string command;

		public AudioPlayerProcess(string command)
		{
			this.command = command;
		}

		public void Play(AudioFile audioFile)
		{
			var tokens = Utility.SplitCommand(command);
			var executable = tokens.First();

			var arguments = new List<string>(tokens.Skip(1)) { audioFile.Path };


			var message = $"Now playing '{audioFile.Title}'";
			if (!string.IsNullOrWhiteSpace(audioFile.Artist))
			{
				message += $" by '{audioFile.Artist}'";
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"{message}...");
			Console.ResetColor();

			_ = Cli.Wrap(executable)
				.WithArguments(arguments)
				.ExecuteAsync();

		}
	}
}
