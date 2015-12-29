using System.Linq;
using Takenet.Text.Types;

namespace Takenet.Text.Scorers
{
    public class MaxMatchesExpressionScorer : IExpressionScorer
    {
        public decimal GetScore(Expression expression)
        {
            // TextTokenType is too wide to be accounted on the input score
            return expression.Tokens.Count(t => t != null) +
                   expression.Tokens.Count(
                       t => t != null && t.Source == TokenSource.Input && !(t.Type is TextTokenType));
        }
    }
}