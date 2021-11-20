using Andtech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Andtech
{

    internal class AudioFileComparer
    {
        private readonly Query query;
        private readonly Regex titleRegex;
        private readonly Regex artistRegex;
        private readonly Regex albumRegex;

        public AudioFileComparer(Query query)
        {
            this.query = query;

            titleRegex = GetRegexFromPrefixes(Utility.Tokenize(query.Title));
            artistRegex = string.IsNullOrEmpty(query.Artist) ? null : GetRegexFromPrefixes(Utility.Tokenize(query.Artist));
            albumRegex = string.IsNullOrEmpty(query.Album) ? null : GetRegexFromPrefixes(Utility.Tokenize(query.Album));
        }

        public bool IsMatch(AudioFile audioFile)
        {
            var title = Utility.Standardize(audioFile.Title);
            var artist = Utility.Standardize(audioFile.Artist);
            var album = Utility.Standardize(audioFile.Album);

            if (!titleRegex.IsMatch(title))
            {
                return false;
            }

            if (artistRegex != null)
            {
                if (string.IsNullOrEmpty(artist))
                {
                    return false;
                }

                if (!artistRegex?.IsMatch(artist) ?? false)
                {
                    return false;
                }
            }

            if (albumRegex != null)
            {
                if (string.IsNullOrEmpty(album))
                {
                    return false;
                }
            }

            if (!albumRegex?.IsMatch(album) ?? false)
            {
                return false;
            }

            return true;
        }

        private static Regex GetRegexFromPrefixes(IEnumerable<string> prefixes)
        {
            var terms = prefixes.Select(x => $@"\b{x}[^\s]*");
            return new Regex($"{string.Join(@"\s+([^\s]+\s+)*", terms)}", RegexOptions.IgnoreCase);
        }
    }
}
