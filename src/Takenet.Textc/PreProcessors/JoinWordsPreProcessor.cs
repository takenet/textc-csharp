using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.PreProcessors
{
    public class JoinWordsPreprocessor : ITextPreprocessor
    {
        private static readonly Regex SpacesBetweenWordsExpression = new Regex(@"\s", RegexOptions.None);


        public JoinWordsPreprocessor()
            : this(0)
        {
        }

        public JoinWordsPreprocessor(int priority)
        {
            Priority = priority;
        }

        public Task<string> ProcessTextAsync(string text, IRequestContext context, CancellationToken cancellationToken)
        {
            text = SpacesBetweenWordsExpression.Replace(text, "");
            return Task.FromResult(text);
        }

        public int Priority { get; }
    }
}