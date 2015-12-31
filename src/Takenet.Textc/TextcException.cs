using System;

namespace Takenet.Textc
{
    /// <summary>
    /// Base class for the library exceptions.
    /// </summary>
    public abstract class TextcException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextcException" /> class.
        /// </summary>
        protected TextcException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextcException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected TextcException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextcException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference (Nothing in
        /// Visual Basic) if no inner exception is specified.
        /// </param>
        protected TextcException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}