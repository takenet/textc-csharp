using System.Collections.Generic;

namespace Takenet.Textc
{
    /// <summary>
    /// Defines a service for splitting a text input into multiple strings.
    /// </summary>
    public interface ITextSplitter
    {
        /// <summary>
        /// Splits an input text into multiple sentences for processing.
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        IEnumerable<string> Split(string inputText);
    }
}
