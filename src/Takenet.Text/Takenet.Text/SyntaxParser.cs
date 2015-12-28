namespace Takenet.Text
{
    /// <summary>
    /// The default syntax parser implementation.
    /// </summary>
    public class SyntaxParser : ISyntaxParser
    {
        public bool TryParse(ITextCursor textCursor, Syntax syntax, IRequestContext context, out Expression expression)
        {
            var queryMatch = true;
            var tokens = new Token[syntax.TokenTemplates.Length];
            var leftPos = 0;
            var rightPos = syntax.TokenTemplates.Length - 1;

            while (leftPos <= rightPos)
            {
                var pos = textCursor.RightToLeftParsing ? rightPos : leftPos;
                var tokenTemplate = syntax.TokenTemplates[pos];

                if (tokenTemplate != null)
                {
                    textCursor.SavePosition();

                    object queryToken;
                    if (tokenTemplate.TryGetTokenFromInput(textCursor, out queryToken))
                    {
                        tokens[pos] = new Token(queryToken, TokenSource.Input, tokenTemplate);
                    }
                    else if (context != null &&
                             tokenTemplate.TryGetTokenFromContext(context, out queryToken))
                    {
                        textCursor.RollbackPosition();
                        tokens[pos] = new Token(queryToken, TokenSource.Context, tokenTemplate);
                    }
                    else if (!tokenTemplate.IsOptional)
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

                if (tokenTemplate != null &&
                    tokenTemplate.InvertParsing)
                {
                    textCursor.InvertParsing();
                }
            }

            var remainingText = textCursor.All();

            if (queryMatch &&
                (!syntax.PerfectMatchOnly || string.IsNullOrEmpty(remainingText)))
            {
                expression = new Expression(tokens, remainingText, syntax, context);
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