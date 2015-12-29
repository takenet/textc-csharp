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
        /// Adds a text pre processor instance.
        /// </summary>
        /// <param name="textPreProcessor">The text pre processor instance to be added.</param>
        void AddTextPreProcessor(ITextPreProcessor textPreProcessor);

        /// <summary>
        /// Adds a command processor.
        /// </summary>
        /// <param name="commandProcessor">The command processor instance to be added.</param>
        void AddCommandProcessor(ICommandProcessor commandProcessor);

        /// <summary>
        /// Processes an input text.
        /// </summary>
        /// <param name="inputText">The user input text.</param>
        /// <param name="context">The request context information.</param>
        /// <returns></returns>
        Task ProcessAsync(string inputText, IRequestContext context);
    }
}