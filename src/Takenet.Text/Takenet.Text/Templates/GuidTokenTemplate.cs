using System;
using Takenet.Text.Metadata;

namespace Takenet.Text.Templates
{
    /// <summary>
    /// Represents a Global Unique Id token template.
    /// </summary>
    [TokenTemplate(ShortName = "Guid")]
    public class GuidTokenTemplate : ValueTokenTemplateBase<Guid>
    {
        public GuidTokenTemplate(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }
    }
}