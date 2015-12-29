using System;
using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    [TokenType(ShortName = "Word")]
    public class WordTokenType : ValueTokenTypeBase<string>
    {
        public WordTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        protected override bool Compare(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }
    }
}