using System.Threading.Tasks;

namespace Takenet.Text.PreProcessors
{
    public class ToLowerCasePreProcessor : ITextPreProcessor
    {
        public ToLowerCasePreProcessor()
            : this(0)
        {
        }

        public ToLowerCasePreProcessor(int priority)
        {
            Priority = priority;
        }

        public Task<string> ProcessTextAsync(string text, IRequestContext context)
        {
            return Task.FromResult(text.ToLowerInvariant());
        }

        public int Priority { get; }
    }
}