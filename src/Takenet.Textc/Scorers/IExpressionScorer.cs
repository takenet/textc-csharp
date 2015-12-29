namespace Takenet.Textc.Scorers
{
    public interface IExpressionScorer
    {
        decimal GetScore(Expression expression);
    }
}