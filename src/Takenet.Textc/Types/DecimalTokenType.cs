using Takenet.Textc.Metadata;

namespace Takenet.Textc.Types
{
    /// <summary>
    /// Represents a decimal number token type.
    /// </summary>
    [TokenType(ShortName = "Decimal")]
    public class DecimalTokenType : ValueTokenTypeBase<decimal>
    {
        public DecimalTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }
    }
}