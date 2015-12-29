using System;
using Takenet.Textc.Types;

namespace Takenet.Textc.Csdl
{
    /// <summary>
    /// Defines a factory for token types.
    /// </summary>
    public interface ITokenTypeFactory
    {
        ITokenType Create(Type tokenType, string name, bool isContextual, bool isOptional, bool invertParsing);
    }
}