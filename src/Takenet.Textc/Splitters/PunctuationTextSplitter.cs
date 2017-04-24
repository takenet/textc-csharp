using System;
using System.Collections.Generic;

namespace Takenet.Textc.Splitters
{
    public class PunctuationTextSplitter : ITextSplitter
    {
        public static string[] SplitMarkers = new[]
        {
            ",",
            ";",
            ".",
            "!",
            ":",
            "?"
        };

        public IEnumerable<string> Split(string inputText) 
            => inputText?.Split(SplitMarkers, StringSplitOptions.RemoveEmptyEntries);
    }
}
