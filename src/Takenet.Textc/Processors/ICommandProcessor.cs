using System.Threading.Tasks;

namespace Takenet.Textc.Processors
{
    /// <summary>
    /// Defines expression processor service.
    /// </summary>
    public interface ICommandProcessor
    {
        /// <summary>
        /// Gets the associated <see cref="IOutputProcessor" /> instance.
        /// </summary>
        IOutputProcessor OutputProcessor { get; }

        /// <summary>
        /// Gets the processor associated syntaxes.
        /// </summary>
        Syntax[] Syntaxes { get; }

        /// <summary>
        /// Processes an expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task ProcessAsync(Expression expression);
    }
}