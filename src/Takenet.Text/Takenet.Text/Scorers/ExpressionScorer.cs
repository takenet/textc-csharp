using System.Linq;

namespace Takenet.Text.Scorers
{
    public class ExpressionScorer : IExpressionScorer
    {
        public decimal GetScore(Expression expression)
        {
            // +1 for each hit
            // +1 if not optional
            // +1 if was from the input
            decimal maxSyntaxMatchPoints = expression.Syntax.TokenTemplates.Count()*2 +
                                           expression.Syntax.TokenTemplates.Count(t => !t.IsOptional);

            decimal expressionMatchPoints =
                expression.Tokens.Count(t => t != null) +
                expression.Tokens.Count(t => t != null && t.Source == TokenSource.Input) +
                expression.Tokens.Count(t => t != null && !t.Template.IsOptional);

            return expressionMatchPoints/maxSyntaxMatchPoints;
        }
    }
}