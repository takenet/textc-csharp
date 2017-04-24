using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;

namespace Takenet.Textc
{
    /// <summary>
    /// Defines a text processor service that processes the user inputs based on the syntaxes defined in the command processors.
    /// </summary>
    public interface ITextProcessor
    {        
        /// <summary>
        /// Gets the collection of registered command processors.
        /// </summary>
        ICollection<ICommandProcessor> CommandProcessors { get; }

        /// <summary>
        /// Gets the collection of registered text preprocessors.
        /// </summary>
        ICollection<ITextPreprocessor> TextPreprocessors { get; }

        /// <summary>
        /// Processes an input text using the registered command processors.
        /// </summary>
        /// <param name="inputText">The user input text.</param>
        /// <param name="context">The request context information.</param>
        /// <param name="cancellationToken">The cancellation token for the execution task.</param>
        /// <returns></returns>       
        Task ProcessAsync(string inputText, IRequestContext context, CancellationToken cancellationToken);
    }
}