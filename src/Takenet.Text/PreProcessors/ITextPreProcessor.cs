using System.Threading.Tasks;

namespace Takenet.Text.PreProcessors
{
    /// <summary>
    /// Defines a text pre processor service that allow the input to be changed before be parsed with a syntax.
    /// </summary>
    public interface ITextPreProcessor
    {
        /// <summary>
        /// Gets the processor priority relative to other pre processors.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Processes the text.
        /// </summary>
        Task<string> ProcessTextAsync(string text, IRequestContext context);
    }
}