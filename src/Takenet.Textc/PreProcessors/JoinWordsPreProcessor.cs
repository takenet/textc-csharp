using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Takenet.Textc.PreProcessors
{
    public class JoinWordsPreProcessor : ITextPreProcessor
    {
        private static readonly Regex SpacesBetweenWordsExpression = new Regex(@"\s", RegexOptions.None);


        public JoinWordsPreProcessor()
            : this(0)
        {
        }

        public JoinWordsPreProcessor(int priority)
        {
            Priority = priority;
        }

        public Task<string> ProcessTextAsync(string text, IRequestContext context)
        {
            text = SpacesBetweenWordsExpression.Replace(text, "");
            return Task.FromResult(text);
        }

        public int Priority { get; }
    }
}