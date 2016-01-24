using System.Linq;
using Takenet.Textc.Types;

namespace Takenet.Textc.Scorers
{
    /// <summary>
    /// Scores by counting the matched tokens in the expression.
    /// </summary>
    public sealed class MatchCountExpressionScorer : IExpressionScorer
    {    
        private readonly bool _ignoresTextTokenMatches;

        public MatchCountExpressionScorer(bool ignoresTextTokenMatches = true)
        {
            _ignoresTextTokenMatches = ignoresTextTokenMatches;
        }

        public decimal GetScore(Expression expression)
        {            
            return expression.Tokens.Count(t => 
                t != null && 
                (!_ignoresTextTokenMatches || !(t.Type is TextTokenType || t.Type is RegexTextTokenType)));
        }
    }
}