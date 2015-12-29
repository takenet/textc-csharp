using System.Text.RegularExpressions;
using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    public abstract class RegexTokenTypeBase<T> : TokenType<T>
    {
        protected Regex Expression;
        private string _pattern;

        protected RegexTokenTypeBase(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        [TokenTypeProperty(IsDefault = true)]
        public string Pattern
        {
            get { return _pattern; }
            set
            {
                _pattern = value;
                Expression = new Regex(_pattern, RegexOptions.None);
            }
        }

        public override bool TryGetTokenFromInput(ITextCursor textCursor, out T token)
        {
            var tokenText = textCursor.Next();

            var match = TryParse(tokenText, out token) &&
                        Expression != null &&
                        Expression.IsMatch(tokenText);

            return match;
        }

        public override bool TryGetTokenFromContext(IRequestContext context, out T token)
        {
            var match = false;
            token = default(T);

            var contextTokenValue = context.GetVariable(Name);

            if (contextTokenValue is T)
            {
                var tokenText = contextTokenValue.ToString();
                var tokenValue = (T) contextTokenValue;

                match = Expression != null &&
                        Expression.IsMatch(tokenText);

                if (match)
                {
                    token = tokenValue;
                }
            }

            return match;
        }

        public abstract bool TryParse(string token, out T value);
    }
}