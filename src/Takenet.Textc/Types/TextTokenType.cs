using System;
using System.Linq;
using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    [TokenType(ShortName = "Text")]
    public class TextTokenType : TokenType<string>
    {
        public TextTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        [TokenTypeProperty(IsDefault = true)]
        public string[] ValidValues { get; internal set; }

        [TokenTypeProperty]
        public string ContextVariableName { get; internal set; }

        public override bool TryGetTokenFromInput(ITextCursor textCursor, out string token)
        {
            var match = false;
            token = null;

            if (ValidValues != null &&
                ValidValues.Length > 0)
            {
                string matchText = null;

                var queryText = string.Empty;

                while (string.IsNullOrEmpty(matchText) &&
                       ValidValues.Any(v => v.Length >= queryText.Length))
                {
                    var tokenPeek = textCursor.Next();

                    if (string.IsNullOrWhiteSpace(tokenPeek)) break;

                    queryText = textCursor.RightToLeftParsing 
                        ? $"{tokenPeek} {queryText}" 
                        : $"{queryText} {tokenPeek}";

                    matchText = TryGetMatchText(ValidValues, queryText.Trim());
                }

                if (!string.IsNullOrEmpty(matchText))
                {
                    token = matchText;
                    match = true;
                }
            }
            else
            {
                var queryText = textCursor.All();

                if (!string.IsNullOrEmpty(queryText))
                {
                    token = queryText;
                    match = true;
                }
            }

            return match;
        }

        protected virtual string TryGetMatchText(string[] matchTexts, string queryText)
        {
            string matchText = null;
            if (matchTexts.Any(t => t.Equals(queryText, StringComparison.InvariantCultureIgnoreCase)))
            {
                matchText = queryText;
            }
            return matchText;
        }

        public override bool TryGetTokenFromContext(IRequestContext context, out string token)
        {
            var match = false;
            token = null;

            var variableName = Name;

            if (!string.IsNullOrWhiteSpace(ContextVariableName))
            {
                variableName = ContextVariableName;
            }

            var objTokenValue = context.GetVariable(variableName);

            if (objTokenValue != null)
            {
                var tokenValue = (string) objTokenValue;

                if (ValidValues != null &&
                    ValidValues.Length > 0)
                {
                    if (
                        ValidValues.Any(
                            w => w.ToString().Equals(tokenValue.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        token = tokenValue;
                        match = true;
                    }
                }
                else
                {
                    token = tokenValue;
                    match = true;
                }
            }

            return match;
        }
    }
}