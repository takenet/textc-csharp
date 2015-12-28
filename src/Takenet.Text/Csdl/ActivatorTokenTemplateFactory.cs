using System;
using Takenet.Text.Templates;

namespace Takenet.Text.Csdl
{
    public class ActivatorTokenTemplateFactory : ITokenTemplateFactory
    {
        public ITokenTemplate Create(Type tokenTemplateType, string name, bool isContextual, bool isOptional,
            bool invertParsing)
        {
            return
                (ITokenTemplate)
                    Activator.CreateInstance(tokenTemplateType, name, isContextual, isOptional, invertParsing);
        }
    }
}