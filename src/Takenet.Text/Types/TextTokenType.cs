using System;
using System.Collections.Generic;
using System.Linq;
using Takenet.Text.Metadata;

namespace Takenet.Text.Types
{
    [TokenType(ShortName = "Text")]
    public class TextTokenType : TokenType<string>
    {
        public TextTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        public string[] ValidValues { get; internal set; }

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

                while (true)
                {
                    var tokenPeek = textCursor.Peek();

                    if (string.IsNullOrWhiteSpace(tokenPeek))
                    {
                        break;
                    }

                    if (textCursor.RightToLeftParsing)
                    {
                        queryText = $"{tokenPeek} {queryText}";
                    }
                    else
                    {
                        queryText = $"{queryText} {tokenPeek}";
                    }

                    queryText = queryText.Trim();

                    string[] matchTexts;

                    if (textCursor.RightToLeftParsing)
                    {
                        matchTexts = ValidValues
                            .Where(t => t.EndsWith(queryText, StringComparison.InvariantCultureIgnoreCase))
                            .OrderByDescending(t => t.Length)
                            .ToArray();
                    }
                    else
                    {
                        matchTexts = ValidValues
                            .Where(t => t.StartsWith(queryText, StringComparison.InvariantCultureIgnoreCase))
                            .OrderByDescending(t => t.Length)
                            .ToArray();
                    }

                    if (matchTexts.Length >= 1)
                    {
                        // Avança o cursor
                        textCursor.Next();

                        if (matchTexts.Any(t => t.Equals(queryText, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            matchText = queryText;
                        }
                    }
                    else
                    {
                        break;
                    }
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