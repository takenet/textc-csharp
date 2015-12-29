namespace Takenet.Textc
{
    /// <summary>
    /// Defines a cursor service that extracts token from an input text.
    /// </summary>
    public interface ITextCursor
    {
        /// <summary>
        /// Gets the context where the cursor was created.
        /// </summary>
        IRequestContext Context { get; }

        /// <summary>
        /// Gets the current parse direction.
        /// </summary>
        bool RightToLeftParsing { get; set; }

        /// <summary>
        /// Indicates if the cursor is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Extracts the current token and advances the cursor.
        /// </summary>
        /// <returns></returns>
        string Next();

        /// <summary>
        /// Gets all remaining text from the cursor.
        /// </summary>
        /// <returns></returns>
        string All();

        /// <summary>
        /// Saves the current cursor position to allow further rollback.
        /// </summary>
        void SavePosition();

        /// <summary>
        /// Rollbacks the cursor to the last saved position.
        /// </summary>
        void RollbackPosition();

        /// <summary>
        /// Gets a preview of the next available token without advancing the cursor position.
        /// </summary>
        string Peek();

        /// <summary>
        /// Inverts the cursor parse direction.
        /// </summary>
        void InvertParsing();

        /// <summary>
        /// Returns to the initial position.
        /// </summary>
        void Reset();
    }
}