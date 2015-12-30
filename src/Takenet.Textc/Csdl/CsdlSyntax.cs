using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Takenet.Textc.Types;

namespace Takenet.Textc.Csdl
{
    /// <summary>
    /// Represents a CSDL syntax instance.
    /// </summary>
    public class CsdlSyntax
    {
        public const char ANCHOR_TOKEN = '^';
        public const char PERFECT_MATCH_START = '[';
        public const char PERFECT_MATCH_END = ']';

        public CsdlSyntax(string syntaxText)
        {
            if (string.IsNullOrWhiteSpace(syntaxText))
            {
                throw new ArgumentNullException(nameof(syntaxText));
            }

            SyntaxText = syntaxText;
            syntaxText = syntaxText.Trim();

            // Parse anchor (direction)
            if (syntaxText[0] == ANCHOR_TOKEN)
            {
                RightToLeftParsing = false;
                syntaxText = syntaxText.TrimStart(ANCHOR_TOKEN);
            }
            else if (syntaxText[syntaxText.Length - 1] == ANCHOR_TOKEN)
            {
                RightToLeftParsing = true;
                syntaxText = syntaxText.TrimEnd(ANCHOR_TOKEN);
            }

            // Check for perfect match
            if (syntaxText.Length > 0 &&
                syntaxText[0] == PERFECT_MATCH_START &&
                syntaxText[syntaxText.Length - 1] == PERFECT_MATCH_END)
            {
                PerfectMatchOnly = true;
                syntaxText = syntaxText.TrimStart(PERFECT_MATCH_START).TrimEnd(PERFECT_MATCH_END);
            }

            Tokens = CsdlToken.GetTokensFromPattern(syntaxText.Trim());
        }

        public CsdlSyntax(bool rightToLeftParsing, bool perfectMatchOnly, CsdlToken[] tokens)
        {
            RightToLeftParsing = rightToLeftParsing;
            PerfectMatchOnly = perfectMatchOnly;

            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            Tokens = tokens;

            var syntaxTextBuilder = new StringBuilder();

            foreach (var csdlToken in tokens)
            {
                syntaxTextBuilder.AppendFormat("{0} ", csdlToken.TokenText);
            }

            SyntaxText = syntaxTextBuilder.ToString().Trim();

            if (PerfectMatchOnly)
            {
                SyntaxText = $"[{SyntaxText}]";
            }

            if (RightToLeftParsing)
            {
                SyntaxText = $"{SyntaxText}^";
            }
        }

        public string SyntaxText { get; }

        public bool RightToLeftParsing { get; }

        public bool PerfectMatchOnly { get; }

        public CsdlToken[] Tokens { get; }

        /// <summary>
        /// Converts to a <see cref="Syntax" /> instance, using the provided token types.
        /// </summary>
        /// <param name="tokenTypeTypeDictionary">The token type dictionary.</param>
        /// <param name="culture">The syntax culture.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">syntaxPattern</exception>
        internal Syntax GetSyntax(IDictionary<string, Type> tokenTypeTypeDictionary, CultureInfo culture)
        {
            var tokenTypes = new ITokenType[Tokens.Count()];

            for (var i = 0; i < Tokens.Count(); i++)
            {
                var syntaxToken = Tokens[i];

                if (tokenTypes.Any(t => t != null && t.Name.Equals(syntaxToken.Name)))
                {
                    throw new InvalidOperationException(
                        $"The token name '{syntaxToken.Name}' is duplicated in the syntax definition");
                }

                tokenTypes[i] = syntaxToken.ToTokenType(tokenTypeTypeDictionary);
            }

            var syntax = new Syntax(tokenTypes, RightToLeftParsing, PerfectMatchOnly, culture);
            return syntax;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return SyntaxText;
        }
    }
}