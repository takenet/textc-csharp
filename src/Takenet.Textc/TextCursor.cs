using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Takenet.Textc
{
    /// <summary>
    /// Defines a cursor that extracts a token for each word in a text.
    /// </summary>
    public sealed class TextCursor : ITextCursor
    {
        public const char TOKEN_SEPARATOR = ' ';

        private readonly string[] _tokens;
        private int _leftPos;
        private int _leftPosCheckpoint;
        private int _rightPos;
        private int _rightPosCheckpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCursor" /> class.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">inputText</exception>
        public TextCursor(string inputText, IRequestContext context)
        {
            if (inputText == null) throw new ArgumentNullException(nameof(inputText));
            _tokens = Regex.Matches(inputText, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value.Replace("'",""))
                .ToArray();
            Context = context;
            Reset();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (var i = _leftPos; i <= _rightPos; i++)
            {
                stringBuilder.Append(_tokens[i]);

                if (i + 1 <= _rightPos)
                {
                    stringBuilder.Append(TOKEN_SEPARATOR);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the context where the cursor was created.
        /// </summary>
        public IRequestContext Context { get; }

        /// <summary>
        /// Extracts the current token and advances the cursor.
        /// </summary>
        /// <returns></returns>
        public string Next()
        {
            var token = Peek();

            if (!string.IsNullOrEmpty(token))
            {
                if (RightToLeftParsing)
                {
                    _rightPos--;
                }
                else
                {
                    _leftPos++;
                }
            }

            return token;
        }

        /// <summary>
        /// Gets all remaining text from the cursor.
        /// </summary>
        /// <returns></returns>
        public string All()
        {
            var text = ToString();
            if (RightToLeftParsing)
            {
                _rightPos = _leftPos - 1;
            }
            else
            {
                _leftPos = _rightPos + 1;
            }

            return text;
        }

        /// <summary>
        /// Saves the current cursor position to allow further rollback.
        /// </summary>
        public void SavePosition()
        {
            _leftPosCheckpoint = _leftPos;
            _rightPosCheckpoint = _rightPos;
        }

        /// <summary>
        /// Rollbacks the cursor to the last saved position.
        /// </summary>
        public void RollbackPosition()
        {
            _leftPos = _leftPosCheckpoint;
            _rightPos = _rightPosCheckpoint;
        }

        /// <summary>
        /// Gets a preview of the next available token without advancing the cursor position.
        /// </summary>
        /// <returns></returns>
        public string Peek()
        {
            string token = null;

            if (!IsEmpty)
            {
                var pos = RightToLeftParsing ? _rightPos : _leftPos;
                token = _tokens[pos];
            }

            return token;
        }

        /// <summary>
        /// Gets the current parse direction.
        /// </summary>
        public bool RightToLeftParsing { get; set; }

        /// <summary>
        /// Inverts the cursor parse direction.
        /// </summary>
        public void InvertParsing()
        {
            RightToLeftParsing = !RightToLeftParsing;
        }

        /// <summary>
        /// Returns to the initial position.
        /// </summary>
        public void Reset()
        {
            _leftPos = _leftPosCheckpoint = 0;
            _rightPos = _rightPosCheckpoint = _tokens.Length - 1;
        }

        /// <summary>
        /// Indicates if the cursor is empty.
        /// </summary>
        public bool IsEmpty => _leftPos > _rightPos;
    }
}