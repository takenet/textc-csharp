using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.PreProcessors
{
    public class ToLowerCasePreprocessor : ITextPreprocessor
    {
        public ToLowerCasePreprocessor()
            : this(0)
        {
        }

        public ToLowerCasePreprocessor(int priority)
        {
            Priority = priority;
        }

        public Task<string> ProcessTextAsync(string text, IRequestContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(text.ToLowerInvariant());
        }

        public int Priority { get; }
    }
}