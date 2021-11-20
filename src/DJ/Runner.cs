using Andtech.Models;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Andtech
{

    internal class Runner
    {
        private readonly Options options;
        private bool Verbose { get; set; }
        private readonly string musicDirectory;

        public Runner(Options options)
        {
            this.options = options;
            Verbose = options.Verbose;

            musicDirectory = Environment.GetEnvironmentVariable("XDG_MUSIC_DIR");
            musicDirectory = Directory.Exists(musicDirectory) ? musicDirectory : Environment.CurrentDirectory;
        }

        public async Task List()
        {
            var results = GetRankedAudioFiles();

            if (results.Any())
            {
                foreach (var result in results.OrderByDescending(x => x.Score).Take(5))
                {
                    Console.WriteLine($"{result.Score}\t{result.AudioFile.Title}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"No matches");
            }
        }

        public async Task Play()
        {
            var results = GetRankedAudioFiles();

            if (results.Any())
            {
                var best = results.OrderByDescending(x => x.Score).First();

                if (!options.DryRun)
                {
                    var player = Environment.GetEnvironmentVariable("PLAYER");
                    var process = new AudioPlayerProcess(player);
                    process.Play(best.AudioFile);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"No matches");
            }
        }

        void Log(object message, bool always = false)
        {
            if (Verbose || always)
            {
                Console.WriteLine(message);
            }
        }

        private IEnumerable<RankResult> GetRankedAudioFiles()
        {
            var searcher = new AudioFileSearcher(musicDirectory);
            var query = Query.Parse(options.Artist, options.Album, options.Tokens.ToArray());
            System.Console.WriteLine($"Title is: {query.Title}");
            System.Console.WriteLine($"Artist is: {query.Artist}");
            System.Console.WriteLine($"Album is: {query.Album}");

            return searcher.GetRanking(query);
        }
    }
}
