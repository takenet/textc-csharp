using Takenet.Text.Metadata;

namespace Takenet.Text.Types
{
    [TokenType(ShortName = "Long")]
    public class LongTokenType : ValueTokenTypeBase<long>
    {
        public LongTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }
    }
}