namespace Takenet.Text.Scorers
{
    public interface IExpressionScorer
    {
        decimal GetScore(Expression expression);
    }
}