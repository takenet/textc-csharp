using System;
using System.Linq;
using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    [TokenType(ShortName = "LDText")]
    public class LevenshteinDistanceTextTokenType : TextTokenType
    {
        public LevenshteinDistanceTextTokenType(string name, bool isContextual, bool isOptional, bool invertParsing) 
            : base(name, isContextual, isOptional, invertParsing)
        {
            MaxDistance = 2;
        }

        [TokenTypeProperty]
        public int MaxDistance { get; internal set; }

        protected override string TryGetMatchText(string[] matchTexts, string queryText) 
            => Array.Find(matchTexts, t => t.CalculateLevenshteinDistance(queryText) <= MaxDistance);
    }
}