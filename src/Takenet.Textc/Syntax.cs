using System;
using System.Globalization;
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
        /// <param name="rightToLeftParsing">Indicates if the parsing should be done from the right to the left direction..</param>
        /// <param name="perfectMatchOnly">Indicates if the syntax requires that all input should be consumed in order to satisfy it.</param>
        /// <param name="culture">The syntax culture.</param>
        public Syntax(ITokenType[] tokenTypes, bool rightToLeftParsing, bool perfectMatchOnly, CultureInfo culture)
        {
            if (tokenTypes == null) throw new ArgumentNullException(nameof(tokenTypes));
            if (tokenTypes.Length == 0) throw new ArgumentException("A syntax must contains at least one token type", nameof(tokenTypes));
            if (culture == null) throw new ArgumentNullException(nameof(culture));

            TokenTypes = tokenTypes;
            RightToLeftParsing = rightToLeftParsing;
            PerfectMatchOnly = perfectMatchOnly;
            Culture = culture;
        }

        /// <summary>
        /// Gets the syntax token types.
        /// </summary>
        public ITokenType[] TokenTypes { get; }

        /// <summary>
        /// Indicates if the parsing should be done from the right to the left direction.
        /// </summary>
        public bool RightToLeftParsing { get; }

        /// <summary>
        /// Indicates if the syntax requires that all input should be consumed in order to satisfy it.
        /// </summary>
        public bool PerfectMatchOnly { get; }

        /// <summary>
        /// The syntax culture. If the culture is <see cref="CultureInfo.InvariantCulture"/>, it will be valid to contexts of any culture.
        /// </summary>        
        public CultureInfo Culture { get; }

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