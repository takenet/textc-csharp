using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Textc.Processors
{
    /// <summary>
    /// Defines expression processor service.
    /// </summary>
    public interface ICommandProcessor
    {
        /// <summary>
        /// Gets the processor associated syntaxes.
        /// </summary>
        Syntax[] Syntaxes { get; }

        /// <summary>
        /// Processes an expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task ProcessAsync(Expression expression, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the associated <see cref="IOutputProcessor" /> instance.
        /// This value can be null.
        /// </summary>
        IOutputProcessor OutputProcessor { get; }
    }
}