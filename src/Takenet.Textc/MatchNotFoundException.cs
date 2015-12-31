namespace Takenet.Textc
{
    /// <summary>
    /// Occurs when there is no match available for the provided input.
    /// </summary>
    public class MatchNotFoundException : TextcException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchNotFoundException" /> class.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        public MatchNotFoundException(string inputText)
            : this(inputText, "Match not found for user input")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchNotFoundException" /> class.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="message">The exception message.</param>
        public MatchNotFoundException(string inputText, string message)
            : base(message)
        {
            InputText = inputText;
        }

        /// <summary>
        /// Gets the input that caused the exception.
        /// </summary>
        public string InputText { get; private set; }
    }
}