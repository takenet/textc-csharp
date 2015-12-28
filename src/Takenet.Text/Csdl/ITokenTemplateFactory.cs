using System;
using Takenet.Text.Templates;

namespace Takenet.Text.Csdl
{
    /// <summary>
    /// Defines a factory for token templates.
    /// </summary>
    public interface ITokenTemplateFactory
    {
        ITokenTemplate Create(Type tokenTemplateType, string name, bool isContextual, bool isOptional, bool invertParsing);
    }
}