using System;

namespace Takenet.Text.Metadata
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TokenTypePropertyAttribute : Attribute
    {
        public bool IsDefault { get; set; }
    }
}