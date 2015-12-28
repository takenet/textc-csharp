using System;
using Takenet.Text.Metadata;

namespace Takenet.Text.Templates
{
    [TokenTemplate(ShortName = "Word")]
    public class WordTokenTemplate : ValueTokenTemplateBase<string>
    {
        public WordTokenTemplate(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        protected override bool Compare(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }
    }
}