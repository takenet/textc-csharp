using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    [TokenType(ShortName = "RegexLong")]
    public class RegexLongTokenType : RegexTokenTypeBase<long>
    {
        public RegexLongTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        public override bool TryParse(string token, out long value)
        {
            return long.TryParse(token, out value);
        }
    }
}