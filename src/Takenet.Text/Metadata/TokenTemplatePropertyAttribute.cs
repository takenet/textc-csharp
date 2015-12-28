using System;

namespace Takenet.Text.Metadata
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TokenTemplatePropertyAttribute : Attribute
    {
        public bool IsDefault { get; set; }
    }
}