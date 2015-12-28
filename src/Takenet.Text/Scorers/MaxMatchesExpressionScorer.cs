using System.Linq;
using Takenet.Text.Templates;

namespace Takenet.Text.Scorers
{
    public class MaxMatchesExpressionScorer : IExpressionScorer
    {
        public decimal GetScore(Expression expression)
        {
            // TextTokenTemplate is too wide to be accounted on the input score
            return expression.Tokens.Count(t => t != null) +
                   expression.Tokens.Count(
                       t => t != null && t.Source == TokenSource.Input && !(t.Template is TextTokenTemplate));
        }
    }
}