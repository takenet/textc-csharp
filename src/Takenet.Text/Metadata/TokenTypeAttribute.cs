using System;
using Takenet.Text.Csdl;

namespace Takenet.Text.Metadata
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TokenTypeAttribute : Attribute
    {
        public TokenTypeAttribute()
        {
            FactoryType = typeof (ActivatorTokenTypeFactory);
        }

        public string ShortName { get; set; }

        public Type FactoryType { get; set; }
    }
}