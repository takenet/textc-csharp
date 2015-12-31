using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.PreProcessors
{
    /// <summary>
    /// Defines a text preprocessor service that allow the input to be changed before be parsed with a syntax.
    /// </summary>
    public interface ITextPreprocessor
    {
        /// <summary>
        /// Gets the processor priority relative to other preprocessors.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Executes the text preprocessing.
        /// </summary>
        Task<string> ProcessTextAsync(string text, IRequestContext context, CancellationToken cancellationToken);
    }
}