using System.Linq;

namespace Takenet.Textc.Scorers
{
    public class ExpressionScorer : IExpressionScorer
    {
        public decimal GetScore(Expression expression)
        {
            // +1 for each hit
            // +1 if not optional
            // +1 if was from the input
            decimal maxSyntaxMatchPoints = expression.Syntax.TokenTypes.Count()*2 +
                                           expression.Syntax.TokenTypes.Count(t => !t.IsOptional);

            decimal expressionMatchPoints =
                expression.Tokens.Count(t => t != null) +
                expression.Tokens.Count(t => t != null && t.Source == TokenSource.Input) +
                expression.Tokens.Count(t => t != null && !t.Type.IsOptional);

            return expressionMatchPoints/maxSyntaxMatchPoints;
        }
    }
}