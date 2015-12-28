using System;
using Takenet.Text.Csdl;

namespace Takenet.Text.Metadata
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TokenTemplateAttribute : Attribute
    {
        public TokenTemplateAttribute()
        {
            FactoryType = typeof (ActivatorTokenTemplateFactory);
        }

        public string ShortName { get; set; }

        public Type FactoryType { get; set; }
    }
}