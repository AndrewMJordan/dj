using Andtech.Models;
using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Console.WriteLine(executable);
            Console.WriteLine(audioFile.Path);

            var arguments = new List<string>(tokens.Skip(1)) { audioFile.Path };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Now playing {audioFile.Title} by {audioFile.Artist}...");
            Console.ResetColor();

            _ = Cli.Wrap(executable)
                .WithArguments(arguments)
                .ExecuteAsync();

        }
    }
}
