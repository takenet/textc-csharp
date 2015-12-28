using Takenet.Text.Metadata;

namespace Takenet.Text.Templates
{
    /// <summary>
    /// Represents an integer number token template.
    /// </summary>
    [TokenTemplate(ShortName = "Integer")]
    public class IntegerTokenTemplate : ValueTokenTemplateBase<int>
    {
        public IntegerTokenTemplate(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }
    }
}