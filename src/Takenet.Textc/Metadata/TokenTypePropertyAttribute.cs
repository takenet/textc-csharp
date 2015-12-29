using System;

namespace Takenet.Textc.Metadata
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TokenTypePropertyAttribute : Attribute
    {
        public bool IsDefault { get; set; }
    }
}