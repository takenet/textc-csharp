using Takenet.Text.Metadata;

namespace Takenet.Text.Templates
{
    [TokenTemplate(ShortName = "WordRegex")]
    public class WordRegexTokenTemplate : RegexTokenTemplateBase<string>
    {
        public WordRegexTokenTemplate(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        public override bool TryParse(string token, out string value)
        {
            value = token;
            return !string.IsNullOrEmpty(token);
        }
    }
}