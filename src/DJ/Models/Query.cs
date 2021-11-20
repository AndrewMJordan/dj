﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Models
{

    internal class Query
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }

        public static Query Parse(string artist, string album, params string[] tokens)
        {
            var input = string.Join(" ", tokens.Select(x => x.Trim()));

            return new Query()
            {
                Title = Utility.Standardize(input),
                Artist = Utility.Standardize(artist),
                Album = Utility.Standardize(album),
            };
        }
    }
}
