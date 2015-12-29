using System.Text;
using Takenet.Textc.Types;

namespace Takenet.Textc
{
    /// <summary>
    /// Defines a text structure with token types that can be used to parse a text input into a <see cref="Expression" />.
    /// </summary>
    public sealed class Syntax
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Syntax" /> class.
        /// </summary>
        /// <param name="tokenTypes">The token types.</param>
        /// <param name="rightToLeftParsing">if set to <c>true</c> [right to left parsing].</param>
        /// <param name="perfectMatchOnly">if set to <c>true</c> [perfect match only].</param>
        public Syntax(ITokenType[] tokenTypes, bool rightToLeftParsing, bool perfectMatchOnly)
        {
            TokenTypes = tokenTypes;
            RightToLeftParsing = rightToLeftParsing;
            PerfectMatchOnly = perfectMatchOnly;
        }

        /// <summary>
        /// Gets the syntax token types.
        /// </summary>
        public ITokenType[] TokenTypes { get; }

        /// <summary>
        /// Indicates if the parsing should be done from the right to the left direction.
        /// </summary>
        public bool RightToLeftParsing { get; private set; }

        /// <summary>
        /// Indicates if the syntax demands that
        /// all input should be consumed in order to satisfy it.
        /// </summary>
        public bool PerfectMatchOnly { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            if (TokenTypes != null)
            {
                foreach (var tokenType in TokenTypes)
                {
                    builder.AppendFormat("{0} ", tokenType);
                }
            }

            return builder.ToString().Trim();
        }
    }
}