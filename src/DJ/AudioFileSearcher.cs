using Andtech.Models;
using FuzzySharp;
using Humanizer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech
{

    internal class RankResult
    {
        public string Term { get; set; }
        public double Score { get; set; }
        public AudioFile AudioFile { get; set; }
    }

    class AudioFileSearcher
    {
        private readonly string searchRoot;

        public AudioFileSearcher(string searchRoot = ".")
        {
            this.searchRoot = searchRoot;
        }

        static List<string> ValidExtensions = new List<string>()
        {
            ".aa",
            ".aax",
            ".aac",
            ".aiff",
            ".ape",
            ".dsf",
            ".flac",
            ".m4a",
            ".m4b",
            ".m4p",
            ".mp3",
            ".mpc",
            ".mpp",
            ".ogg",
            ".oga",
            ".wav",
            ".wma",
            ".wv",
            ".webm",
        };

        public IEnumerable<RankResult> GetRanking(Query query)
        {
            var paths = Directory
                .EnumerateFiles(searchRoot, "*", SearchOption.AllDirectories)
                .Where(IsMusicFile);

            var titleRegex = GetRegexFromPrefixes(Utility.Standardize(query.Title).Split(' '));
            var audioFiles = paths
                .Select(AudioFile.Read);
            var rankings = audioFiles
                .Where(x => titleRegex.IsMatch(x.Title))
                .Select(x => ToData(x));

            return rankings;

            Regex GetRegexFromPrefixes(IEnumerable<string> prefixes)
            {
                var terms = prefixes .Select(x => $@"\b{x}[^\s]*");
                return new Regex($"{string.Join(@"\s+([^\s]+\s+)*", terms)}", RegexOptions.IgnoreCase);
            }

            RankResult ToData(AudioFile audioFile)
            {
                var expected = Utility.Standardize(query.Title);
                var actual = Utility.Standardize(audioFile.Title);
                var score = Fuzz.Ratio(expected, actual, FuzzySharp.PreProcess.PreprocessMode.Full);

                return new RankResult { AudioFile = audioFile, Term = actual, Score = score };
            }
        }

        static bool IsMusicFile(string path)
        {
            return ValidExtensions.Contains(Path.GetExtension(path));
        }
    }
}
