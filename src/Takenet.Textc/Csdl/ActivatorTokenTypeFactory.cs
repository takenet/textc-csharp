using System;
using Takenet.Textc.Types;

namespace Takenet.Textc.Csdl
{
    public class ActivatorTokenTypeFactory : ITokenTypeFactory
    {
        public ITokenType Create(Type tokenType, string name, bool isContextual, bool isOptional,
            bool invertParsing)
        {
            return
                (ITokenType)
                    Activator.CreateInstance(tokenType, name, isContextual, isOptional, invertParsing);
        }
    }
}