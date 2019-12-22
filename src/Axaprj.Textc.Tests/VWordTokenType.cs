using System;
using Takenet.Textc.Metadata;
using Takenet.Textc.Types;

[assembly:TokenTypeLibrary]

namespace Axaprj.Textc.Tests
{
    [TokenType(ShortName = "VWord")]
    public class VWordTokenType : ValueTokenTypeBase<string>
    {
        public VWordTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        protected override bool Compare(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }
    }
}