using Takenet.Text.Metadata;

namespace Takenet.Text.Templates
{
    [TokenTemplate(ShortName = "LongRegex")]
    public class LongRegexTokenTemplate : RegexTokenTemplateBase<long>
    {
        public LongRegexTokenTemplate(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        public override bool TryParse(string token, out long value)
        {
            return long.TryParse(token, out value);
        }
    }
}