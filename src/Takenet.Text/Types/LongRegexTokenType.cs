using Takenet.Text.Metadata;

namespace Takenet.Text.Types
{
    [TokenType(ShortName = "LongRegex")]
    public class LongRegexTokenType : RegexTokenTypeBase<long>
    {
        public LongRegexTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        public override bool TryParse(string token, out long value)
        {
            return long.TryParse(token, out value);
        }
    }
}