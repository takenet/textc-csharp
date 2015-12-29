namespace Takenet.Textc
{
    /// <summary>
    /// Defines a service that parses a user input to a expression.
    /// </summary>
    public interface ISyntaxParser
    {
        /// <summary>
        /// Tries the parse the text cursor content using the provided syntax.
        /// </summary>
        /// <param name="textCursor">The text cursor.</param>
        /// <param name="syntax">The syntax to be used.</param>
        /// <param name="context">The request context.</param>
        /// <param name="expression">The result expression.</param>
        /// <returns>True if the input was parsed successfully; otherwise, false.</returns>
        bool TryParse(ITextCursor textCursor, Syntax syntax, IRequestContext context, out Expression expression);
    }
}