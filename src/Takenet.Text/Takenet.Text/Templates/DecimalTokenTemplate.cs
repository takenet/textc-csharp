using Takenet.Text.Metadata;

namespace Takenet.Text.Templates
{
    /// <summary>
    /// Represents a decimal number token template.
    /// </summary>
    [TokenTemplate(ShortName = "Decimal")]
    public class DecimalTokenTemplate : ValueTokenTemplateBase<decimal>
    {
        public DecimalTokenTemplate(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }
    }
}