namespace Takenet.Textc
{
    /// <summary>
    /// Stores information of an extracted expression from the user input that was parsed with a syntax.
    /// </summary>
    public sealed class Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expression" /> class.
        /// </summary>
        public Expression(Token[] tokens, string remainingText, Syntax syntax, IRequestContext context)
        {
            Tokens = tokens;
            RemainingText = remainingText;
            Syntax = syntax;
            Context = context;
        }

        /// <summary>
        /// Gets the tokens that compose the expression.
        /// </summary>
        public Token[] Tokens { get; }

        /// <summary>
        /// Gets the remaining text that was not processed.
        /// In case of perfect match syntaxes, this value is null.
        /// </summary>
        public string RemainingText { get; }

        /// <summary>
        /// Gets the syntax that originated the expression.
        /// </summary>
        public Syntax Syntax { get; }

        /// <summary>
        /// Gets the expression request context.
        /// </summary>
        public IRequestContext Context { get; }
    }
}