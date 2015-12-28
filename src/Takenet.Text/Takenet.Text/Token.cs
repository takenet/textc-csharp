using Takenet.Text.Templates;

namespace Takenet.Text
{
    /// <summary>
    /// Represents a text token.
    /// </summary>
    public sealed class Token
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="source">The source.</param>
        /// <param name="template">The template.</param>
        public Token(object value, TokenSource source, ITokenTemplate template)
        {
            Value = value;
            Source = source;
            Template = template;
        }

        /// <summary>
        /// Gets the token value.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the source extraction source.
        /// </summary>
        public TokenSource Source { get; private set; }

        /// <summary>
        /// Gets the associated token template.
        /// </summary>
        public ITokenTemplate Template { get; private set; }
    }

    /// <summary>
    /// Indicates the source of the extraction of a token.
    /// </summary>
    public enum TokenSource
    {
        /// <summary>
        /// The token was extracted from the user input.
        /// </summary>
        Input,

        /// <summary>
        /// The token was extracted from the request context.
        /// </summary>
        Context
    }
}