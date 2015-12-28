using Takenet.Text.Metadata;

namespace Takenet.Text.Templates
{
    [TokenTemplate(ShortName = "TextRegex")]
    public class TextRegexTokenTemplate : RegexTokenTemplateBase<string>
    {
        public TextRegexTokenTemplate(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        public override bool TryGetTokenFromInput(ITextCursor textCursor, out string token)
        {
            var tokenText = textCursor.All();

            var match = TryParse(tokenText, out token) &&
                        Expression != null &&
                        Expression.IsMatch(tokenText);

            return match;
        }

        public override bool TryParse(string token, out string value)
        {
            value = token;
            return !string.IsNullOrEmpty(token);
        }
    }
}