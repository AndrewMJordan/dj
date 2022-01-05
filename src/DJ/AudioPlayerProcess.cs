using CliWrap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech.DJ
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
			Utility.RunInDirectory(WorkingDirectory, Play);

			void Play()
			{
				var tokens = Utility.SplitCommand(command);
				var executable = tokens.First();

				Environment.CurrentDirectory = WorkingDirectory;
				var arguments = new List<string>(tokens.Skip(1)) { Path.GetRelativePath(Environment.CurrentDirectory, audioFile.Path) };

				if (Verbose)
				{
					Console.WriteLine($"{executable} {string.Join(" ", arguments)}");
				}

				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"Now playing {audioFile}...");
				Console.ResetColor();

				_ = Cli.Wrap(executable)
					.WithWorkingDirectory(WorkingDirectory)
					.WithArguments(arguments)
					.ExecuteAsync();
			}
		}
	}
}
