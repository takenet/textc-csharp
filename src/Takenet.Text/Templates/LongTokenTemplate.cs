using Takenet.Text.Metadata;

namespace Takenet.Text.Templates
{
    [TokenTemplate(ShortName = "Long")]
    public class LongTokenTemplate : ValueTokenTemplateBase<long>
    {
        public LongTokenTemplate(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }
    }
}