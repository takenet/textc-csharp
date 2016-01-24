using System.Linq;
using Takenet.Textc.Types;

namespace Takenet.Textc.Scorers
{
    /// <summary>
    /// Scores using ratio of the expression matched tokens by the total syntax possible token matches.
    /// </summary>
    public sealed class RatioExpressionScorer : IExpressionScorer
    {
        private readonly bool _ignoresTextTokenMatches;

        public RatioExpressionScorer(bool ignoresTextTokenMatches = true)
        {
            _ignoresTextTokenMatches = ignoresTextTokenMatches;
        }

        public decimal GetScore(Expression expression)
        {
            // +1 for each hit
            // +1 if not optional
            // +1 if was from the input
            // +1 if is text
            decimal maxMatchPoints = 
                expression.Syntax.TokenTypes.Count() * 2 +
                expression.Syntax.TokenTypes.Count(t => !t.IsOptional) +
                expression.Syntax.TokenTypes.Count(t => t is TextTokenType || t is RegexTextTokenType);

            decimal expressionMatchPoints =
                expression.Tokens.Count(t => t != null) +
                expression.Tokens.Count(t => t != null && t.Source == TokenSource.Input) +
                expression.Tokens.Count(t => t != null && !t.Type.IsOptional) +
                (_ignoresTextTokenMatches ? 0 : expression.Tokens.Count(t => t.Type is TextTokenType || t.Type is RegexTextTokenType));

            return expressionMatchPoints / maxMatchPoints;
        }
    }
}