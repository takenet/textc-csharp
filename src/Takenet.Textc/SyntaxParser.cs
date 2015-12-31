namespace Takenet.Textc
{
    /// <summary>
    /// The default syntax parser implementation.
    /// </summary>
    public sealed class SyntaxParser : ISyntaxParser
    {
        public bool TryParse(ITextCursor textCursor, Syntax syntax, IRequestContext context, out Expression expression)
        {
            var queryMatch = true;
            var tokens = new Token[syntax.TokenTypes.Length];
            var leftPos = 0;
            var rightPos = syntax.TokenTypes.Length - 1;

            while (leftPos <= rightPos)
            {
                var pos = textCursor.RightToLeftParsing ? rightPos : leftPos;
                var tokenType = syntax.TokenTypes[pos];

                if (tokenType != null)
                {
                    textCursor.SavePosition();

                    object queryToken;
                    if (tokenType.TryGetTokenFromInput(textCursor, out queryToken))
                    {
                        tokens[pos] = new Token(queryToken, TokenSource.Input, tokenType);
                    }
                    else if (context != null &&
                             tokenType.TryGetTokenFromContext(context, out queryToken))
                    {
                        textCursor.RollbackPosition();
                        tokens[pos] = new Token(queryToken, TokenSource.Context, tokenType);
                    }
                    else if (!tokenType.IsOptional)
                    {
                        queryMatch = false;
                        break;
                    }
                    else
                    {
                        textCursor.RollbackPosition();
                    }
                }

                if (textCursor.RightToLeftParsing)
                {
                    rightPos--;
                }
                else
                {
                    leftPos++;
                }

                if (tokenType != null &&
                    tokenType.InvertParsing)
                {
                    textCursor.InvertParsing();
                }
            }

            var remainingText = textCursor.All();

            if (queryMatch &&
                (!syntax.PerfectMatchOnly || string.IsNullOrEmpty(remainingText)))
            {
                expression = new Expression(tokens, syntax, context, remainingText);
            }
            else
            {
                queryMatch = false;
                expression = null;
            }

            return queryMatch;
        }
    }
}