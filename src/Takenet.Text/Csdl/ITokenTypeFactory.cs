using System;
using Takenet.Text.Types;

namespace Takenet.Text.Csdl
{
    /// <summary>
    /// Defines a factory for token types.
    /// </summary>
    public interface ITokenTypeFactory
    {
        ITokenType Create(Type tokenType, string name, bool isContextual, bool isOptional, bool invertParsing);
    }
}