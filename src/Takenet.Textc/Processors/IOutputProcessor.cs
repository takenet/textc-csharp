using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.Processors
{
    /// <summary>
    /// Defines a output processor that handles the result of a expression processing by a command processor.
    /// </summary>
    public interface IOutputProcessor
    {
        /// <summary>
        /// Processes the given command output.
        /// </summary>
        /// <param name="output">The output object.</param>
        /// <param name="context">The request context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task ProcessOutputAsync(object output, IRequestContext context, CancellationToken cancellationToken);
    }
}